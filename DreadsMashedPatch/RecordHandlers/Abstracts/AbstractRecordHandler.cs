using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.Contexts.Interfaces;
using DreadsMashedPatch.Enums;

namespace DreadsMashedPatch.RecordHandlers.Abstracts
{
    public abstract class AbstractRecordHandler : IRecordHandler
    {
        private static readonly ConcurrentDictionary<string, byte> EmittedFormatterWarnings = new(StringComparer.Ordinal);
        private static readonly ConcurrentDictionary<string, byte> AuditedFormatterTypes = new(StringComparer.Ordinal);

        protected static IReadOnlySet<string> EmptyAtomicOwnershipTriggerProperties { get; } =
            new HashSet<string>(StringComparer.Ordinal);

        // Abstract property that all record handlers must implement
        public abstract Dictionary<string, IPropertyHandler> PropertyHandlers { get; }

        /// <summary>
        /// Properties whose change between adjacent overrides makes the newer override
        /// the complete ownership baseline for this record. Empty by default.
        /// </summary>
        protected virtual IReadOnlySet<string> AtomicOwnershipTriggerProperties =>
            EmptyAtomicOwnershipTriggerProperties;

        protected Dictionary<string, IPropertyContext> PropertyContexts { get; private set; } = [];

        /// <summary>
        /// Gives record-specific semantic groups one final opportunity to keep related
        /// properties coherent after the ordinary per-property conflict decisions.
        /// </summary>
        protected virtual PropertyForwardingCoordination CoordinateForwardedProperties(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts)
        {
            return PropertyForwardingCoordination.None;
        }

        /// <summary>
        /// Applies a narrowly scoped whole-record policy before the ordinary early-exit,
        /// priority-mod, and property-forwarding paths. Returning true means the record
        /// was fully handled, including policies which deliberately emit no override.
        /// </summary>
        protected virtual bool TryApplyRecordPolicy(
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts)
        {
            return false;
        }

        private static bool IsLikelyTypeNameString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            // Common generic/runtime type-name shapes.
            return text.Contains("System.Collections.Generic.", StringComparison.Ordinal)
                || text.Contains("Mutagen.Bethesda.", StringComparison.Ordinal)
                || text.Contains("`1[", StringComparison.Ordinal)
                || text.Contains("`2[", StringComparison.Ordinal);
        }

        private static bool IsLowFidelityFormat(object? value, string formatted)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string)
            {
                return false;
            }

            // Do not call value.ToString() here: formatter auditing must be observational and
            // some generated/runtime values have unsafe or low-fidelity ToString implementations.
            var runtimeType = value.GetType();
            var isBareRuntimeTypeName = string.Equals(formatted, runtimeType.FullName, StringComparison.Ordinal)
                || string.Equals(formatted, runtimeType.Name, StringComparison.Ordinal)
                || string.Equals(formatted, runtimeType.ToString(), StringComparison.Ordinal);
            if (isBareRuntimeTypeName && IsLikelyTypeNameString(formatted))
            {
                return true;
            }

            // Enumerable payloads should usually not format to bare type names.
            if (value is IEnumerable && IsLikelyTypeNameString(formatted))
            {
                return true;
            }

            return false;
        }

        private string FormatForLogWithWarning(
            string propertyName,
            IPropertyHandler handler,
            object? value,
            string stage,
            bool deepDiveRecord)
        {
            var formatted = handler.FormatValue(value);
            EmitLowFidelityWarning(propertyName, handler, value, formatted, stage);

            return LoggingSettings.ForLog(formatted, deepDiveRecord);
        }

        private static void EmitLowFidelityWarning(
            string propertyName,
            IPropertyHandler handler,
            object? value,
            string formatted,
            string stage)
        {
            if (!IsLowFidelityFormat(value, formatted))
            {
                return;
            }

            var warningKey = $"{handler.GetType().FullName}|{propertyName}|{value!.GetType().FullName}";
            if (EmittedFormatterWarnings.TryAdd(warningKey, 0))
            {
                Console.WriteLine($"[Warning] [{propertyName}] {stage}: formatter returned a type-name fallback: {formatted}. Consider overriding FormatValue in {handler.GetType().Name}.");
            }
        }

        private static void AuditFormatterWarnings(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
            IReadOnlyDictionary<string, IPropertyHandler> propertyHandlers)
        {
            foreach (var (propertyName, handler) in propertyHandlers)
            {
                var auditedRuntimeTypes = new HashSet<Type>();
                foreach (var context in recordContexts)
                {
                    var value = handler.GetValue(context.Record);
                    if (value == null || !auditedRuntimeTypes.Add(value.GetType()))
                    {
                        continue;
                    }

                    var auditKey = $"{handler.GetType().FullName}|{propertyName}|{value.GetType().FullName}";
                    if (!AuditedFormatterTypes.TryAdd(auditKey, 0))
                    {
                        continue;
                    }

                    try
                    {
                        var formatted = handler.FormatValue(value);
                        EmitLowFidelityWarning(
                            propertyName,
                            handler,
                            value,
                            formatted,
                            $"formatter audit ({context.ModKey})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Warning] [{propertyName}] formatter audit ({context.ModKey}): FormatValue threw {ex.GetType().Name}: {ex.Message}");
                    }
                }
            }
        }


        /// <summary>
        /// Initialize the property contexts for the record
        /// </summary>
        /// <param name="originalContext"></param>
        /// <param name="winningContext"></param>
        protected void InitializePropertyContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
        {
            PropertyContexts.Clear();
            foreach (var (propertyName, handler) in PropertyHandlers)
            {
                // Let each handler create its own properly typed context
                var propertyContext = handler.CreatePropertyContext();
                PropertyContexts[propertyName] = propertyContext;
                handler.InitializeContext(originalContext, winningContext, propertyContext);
            }
        }

        /// <summary>
        /// Returns the configured atomic trigger properties which changed between two
        /// adjacent override versions, using each property's semantic equality.
        /// </summary>
        protected IReadOnlyList<string> GetChangedAtomicOwnershipTriggerProperties(
            IMajorRecordGetter previousRecord,
            IMajorRecordGetter currentRecord)
        {
            if (AtomicOwnershipTriggerProperties.Count == 0)
            {
                return [];
            }

            var changedProperties = new List<string>();
            foreach (var propertyName in AtomicOwnershipTriggerProperties)
            {
                if (!PropertyHandlers.TryGetValue(propertyName, out var handler))
                {
                    throw new InvalidOperationException(
                        $"Atomic ownership trigger property '{propertyName}' has no registered handler in {GetType().Name}.");
                }

                var previousValue = handler.GetValue(previousRecord);
                var currentValue = handler.GetValue(currentRecord);
                if (!handler.AreValuesEqual(previousValue, currentValue))
                {
                    changedProperties.Add(propertyName);
                }
            }

            return changedProperties;
        }

        /// <summary>
        /// Replaces every property context with the current override's exact snapshot
        /// when an atomic trigger changed. Passing the same context as original and
        /// winning assigns all property ownership to that override.
        /// </summary>
        protected IReadOnlyList<string> ResetPropertyContextsIfAtomicOwnershipTriggered(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> previousContext,
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> currentContext)
        {
            var changedProperties = GetChangedAtomicOwnershipTriggerProperties(
                previousContext.Record,
                currentContext.Record);
            if (changedProperties.Count > 0)
            {
                InitializePropertyContexts(currentContext, currentContext);
            }

            return changedProperties;
        }

        /// <summary>
        /// Process the record
        /// </summary>
        /// <param name="state"></param>
        /// <param name="filteredWinningContexts"></param>
        /// <param name="policySources">Shared run-scoped lookups; direct callers can omit this</param>
        public PatchRunReport Process(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] filteredWinningContexts, RecordPolicySources? policySources = null)
        {
            using var ownedPolicySources = policySources == null ? new RecordPolicySources(state) : null;
            policySources ??= ownedPolicySources!;
            using var runDiagnostics = new PatchDiagnostics();
            foreach (var discoveredWinningContext in filteredWinningContexts)
            {
                using var diagnostics = new PatchDiagnostics(discoveredWinningContext.Record.FormKey);
                try
                {
                    var initialContexts = RecordPolicySources.GetInitialContexts(discoveredWinningContext, state);
                    if (initialContexts.Length == 0) continue;
                    var winningContext = initialContexts[0];
                    var deepDiveRecord = LoggingSettings.IsDeepDiveRecord(winningContext);
                    var detailedRecord = deepDiveRecord || LoggingSettings.Verbosity == PatcherLogVerbosity.Detailed;
                    var auditContextChanges = deepDiveRecord || LoggingSettings.Verbosity != PatcherLogVerbosity.Summary;
                    LogCollector.SetRecordLoggingContext(deepDiveRecord, detailedRecord);

                    Console.WriteLine(new string('-', 80));
                    Console.WriteLine($"Processing: {winningContext.Record.FormKey} ({winningContext.Record.EditorID})");
                    Console.WriteLine($"Record type: {RecordTypeCatalog.GetRecordDescription(winningContext.Record)}");

                    // Shared policy migration: explicit priority snapshots are handled before
                    // vanilla/short-history exits and before loading the merge history.
                    if (policySources.TryGetAlwaysWinningMod(winningContext.Record, out var priorityMod))
                    {
                        if (priorityMod == winningContext.ModKey)
                        {
                            Console.WriteLine($"Always-win source {priorityMod} already wins; no patch record is needed");
                        }
                        else
                        {
                            Console.WriteLine($"Always-win override: copying the complete record from {priorityMod} over {winningContext.ModKey}");
                            CommitOverride(policySources.GetPriorityContext(winningContext.Record, priorityMod), state);
                        }
                        continue;
                    }

                    var needsRecordPolicy = winningContext.Record is ICellGetter
                        && CellRecordHandler.RequiresSmartPolicyProcessing(winningContext.Record.FormKey);
                    if (!needsRecordPolicy && RecordPolicySources.ShouldSkipOrdinaryProcessing(initialContexts))
                    {
                        PreserveBaselineEditorIdIfNeeded(winningContext, state, policySources);
                        continue;
                    }

                    // Full-history resolution remains reserved for ordinary merging
                    // and Tamriel's specialized CELL policy.
                    var recordContexts = GetRecordContexts(winningContext, state)
                        .Where(context => !PatcherSettings.IsIgnoredMod(context.ModKey))
                        .ToArray();
                    if (recordContexts.Length == 0) continue;
                    winningContext = recordContexts[0];
                    if (TryApplyRecordPolicy(state, recordContexts)) continue;
                    if (RecordPolicySources.ShouldSkipOrdinaryProcessing(recordContexts))
                    {
                        PreserveBaselineEditorIdIfNeeded(winningContext, state, policySources);
                        continue;
                    }

                    var hasBaselineEditorId = policySources.TryGetBaselineEditorId(winningContext.Record, out var baselineEditorId);
                    // Formatter diagnostics are warnings, so they must not depend on Detailed logging.
                    AuditFormatterWarnings(recordContexts, PropertyHandlers);

                    Console.WriteLine($"Record contexts: {recordContexts.Length}");
                    Console.WriteLine($"Winning context: {winningContext.ModKey}");

                    // Initialize property states before visiting the complete override history
                    var originalContext = recordContexts.Last();
                    Console.WriteLine($"Original context: {originalContext.ModKey}");
                    var usesAtomicOwnership = AtomicOwnershipTriggerProperties.Count > 0;
                    InitializePropertyContexts(
                        originalContext,
                        usesAtomicOwnership ? originalContext : winningContext);

                    // print original and winning values for all properties
                    foreach (var (propName, handler) in PropertyHandlers)
                    {
                        var originalValue = handler.GetValue(originalContext.Record);
                        var winningValue = handler.GetValue(winningContext.Record);
                        if (detailedRecord)
                        {
                            LogCollector.Add(propName, $"[{propName}] Original: {FormatForLogWithWarning(propName, handler, originalValue, "initial original", deepDiveRecord)}, Winning: {FormatForLogWithWarning(propName, handler, winningValue, "initial winning", deepDiveRecord)}");
                        }
                    }
                    if (LogCollector.HasLogs())
                    {
                        LogCollector.PrintAllAndClear();
                    }

                    // Every handler uses the complete history, from original to winning.
                    // IsResolved still permits record-specific policies to stop early.
                    if (detailedRecord) Console.WriteLine("Processing forward pass");

                    // iterate from original to winning
                    var chronologicalPreviousContext = originalContext;
                    foreach (var context in recordContexts.Reverse().Skip(1))
                    {
                        // bugfix, skip if context is output mod
                        if (context.ModKey.ToString() == state.PatchMod.ModKey.ToString())
                        {
                            continue;
                        }

                        var changedAtomicProperties = ResetPropertyContextsIfAtomicOwnershipTriggered(
                            chronologicalPreviousContext,
                            context);
                        if (changedAtomicProperties.Count > 0)
                        {
                            Console.WriteLine(
                                $"Atomic ownership reset: {context.ModKey} owns the complete record because " +
                                $"{string.Join(", ", changedAtomicProperties)} changed");
                            chronologicalPreviousContext = context;
                            continue;
                        }

                        // Update the property contexts, skip if resolved
                        foreach (var (propName, handler) in PropertyHandlers)
                        {
                            var propContext = PropertyContexts[propName];
                            if (propContext.IsResolved) continue;

                            var mod = state.LoadOrder[context.ModKey].Mod;
                            if (detailedRecord)
                            {
                                LogCollector.Add(propName, $"[{propName}] Processing mod: {context.ModKey} with value: {FormatForLogWithWarning(propName, handler, handler.GetValue(context.Record), "forward-pass context value", deepDiveRecord)} with masters: {(mod != null ? string.Join(", ", mod.MasterReferences.Select(m => m.Master.FileName)) : "")}");
                            }

                            handler.UpdatePropertyContext(context, state, propContext);
                        }

                        chronologicalPreviousContext = context;
                    }

                    if (LogCollector.HasLogs())
                    {
                        LogCollector.PrintAllAndClear();
                    }
                    if (detailedRecord) Console.WriteLine("Forward pass complete");

                    // Forward changes to the patcher
                    var propertiesToForward = new Dictionary<string, object?>();
                    int unchangedDecisionCount = 0;
                    var decisionAuditContexts = recordContexts
                        .Where(context => context.ModKey.ToString() != state.PatchMod.ModKey.ToString())
                        .ToArray();
                    var coordination = CoordinateForwardedProperties(decisionAuditContexts);
                    if (auditContextChanges && coordination.AuditMessage != null)
                    {
                        LogCollector.AddDecisionAudit("UDR", coordination.AuditMessage);
                    }

                    foreach (var kvp in PropertyContexts)
                    {
                        var propertyName = kvp.Key;
                        var propertyContext = kvp.Value;

                        if (propertyContext == null || !PropertyHandlers.TryGetValue(propertyName, out var handler) || handler == null)
                        {
                            continue;
                        }

                        var originalValue = handler.GetValue(originalContext.Record);
                        var winningValue = handler.GetValue(winningContext.Record);
                        var forwardValue = coordination.ForwardValues.TryGetValue(propertyName, out var coordinatedValue)
                            ? coordinatedValue
                            : propertyContext.GetForwardValue();
                        if (propertyName == "EditorID" && hasBaselineEditorId)
                        {
                            // Shared EDID policy: every record family uses the latest configured
                            // official baseline. Record-specific handlers remain otherwise unchanged.
                            forwardValue = baselineEditorId;
                        }
                        var shouldForward = !handler.AreValuesEqual(forwardValue, winningValue);

                        if (shouldForward)
                        {
                            propertiesToForward[propertyName] = forwardValue;
                        }
                        else
                        {
                            unchangedDecisionCount++;
                        }

                        if (!auditContextChanges)
                        {
                            continue;
                        }

                        var contextValues = decisionAuditContexts
                            .Select(context => (Context: context, Value: handler.GetValue(context.Record)))
                            .ToArray();
                        var hasContextChanges = contextValues.Length > 1
                            && contextValues.Skip(1).Any(entry => !handler.AreValuesEqual(contextValues[0].Value, entry.Value));

                        // ContextChanges mode omits properties which are identical throughout the
                        // chain and are not forwarded. Detailed/deep-dive mode can include them all.
                        var includeStableProperty = deepDiveRecord
                            || (detailedRecord && LoggingSettings.IncludeNoChangeDecisionsInDetailed);
                        var shouldLogDecisionBlock = shouldForward || hasContextChanges || includeStableProperty;
                        if (!shouldLogDecisionBlock)
                        {
                            continue;
                        }

                        LogCollector.AddDecisionAudit(propertyName, $"[{propertyName}] Decision audit:");
                        LogCollector.AddDecisionAudit(propertyName, $"[{propertyName}]   Context values (winning -> original):");
                        foreach (var (context, value) in contextValues)
                        {
                            LogCollector.AddDecisionAudit(
                                propertyName,
                                $"[{propertyName}]     {context.ModKey}: {FormatForLogWithWarning(propertyName, handler, value, $"context {context.ModKey}", deepDiveRecord)}");
                        }

                        if (deepDiveRecord && handler is IDiagnosticDiffPropertyHandler diagnosticDiff)
                        {
                            LogCollector.AddDecisionAudit(propertyName, $"[{propertyName}]   Changes (original -> winning):");
                            for (var index = contextValues.Length - 2; index >= 0; index--)
                            {
                                var older = contextValues[index + 1];
                                var newer = contextValues[index];
                                var difference = diagnosticDiff.FormatDifference(older.Value, newer.Value);
                                if (string.Equals(difference, "No semantic changes", StringComparison.Ordinal))
                                {
                                    continue;
                                }

                                LogCollector.AddDecisionAudit(
                                    propertyName,
                                    $"[{propertyName}]     {older.Context.ModKey} -> {newer.Context.ModKey}: {difference}");
                            }

                            var matchingContext = contextValues.FirstOrDefault(entry =>
                                handler.AreValuesEqual(forwardValue, entry.Value));
                            if (matchingContext.Context != null)
                            {
                                LogCollector.AddDecisionAudit(
                                    propertyName,
                                    $"[{propertyName}]   Computed = {matchingContext.Context.ModKey} " +
                                    $"({diagnosticDiff.FormatIdentity(forwardValue)})");
                            }
                            else
                            {
                                LogCollector.AddDecisionAudit(
                                    propertyName,
                                    $"[{propertyName}]   Computed: {FormatForLogWithWarning(propertyName, handler, forwardValue, "final-decision forward", deepDiveRecord)}");
                            }
                        }
                        else
                        {
                            LogCollector.AddDecisionAudit(propertyName, $"[{propertyName}]   Original value: {FormatForLogWithWarning(propertyName, handler, originalValue, "final-decision original", deepDiveRecord)}");
                            LogCollector.AddDecisionAudit(propertyName, $"[{propertyName}]   Winning value: {FormatForLogWithWarning(propertyName, handler, winningValue, "final-decision winning", deepDiveRecord)}");
                            LogCollector.AddDecisionAudit(propertyName, $"[{propertyName}]   Computed value: {FormatForLogWithWarning(propertyName, handler, forwardValue, "final-decision forward", deepDiveRecord)}");
                        }
                        LogCollector.AddDecisionAudit(
                            propertyName,
                            $"[{propertyName}]   Decision: {(shouldForward ? "FORWARD (computed value differs from winning)" : "KEEP WINNING (computed value equals winning)")}");
                    }

                    if (LogCollector.HasLogs())
                    {
                        LogCollector.PrintAllAndClear();
                    }

                    if (PatcherSettings.EditorIdPolicy == EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges
                        && propertiesToForward.Count == 1
                        && propertiesToForward.Remove("EditorID"))
                    {
                        unchangedDecisionCount++;
                        Console.WriteLine("EDID policy: skipped an EDID-only patch record");
                    }

                    Console.WriteLine($"Decision summary: forward {propertiesToForward.Count}, unchanged {unchangedDecisionCount}");

                    if (detailedRecord) Console.WriteLine($"Properties to forward: {propertiesToForward.Count}");
                    diagnostics.Report.ThrowIfFailed();
                    if (propertiesToForward.Count > 0)
                    {
                        CommitOverride(winningContext, state, propertiesToForward);
                    }

                }
                catch (Exception ex)
                {
                    diagnostics.SkipRecord(RecordTypeCatalog.GetRecordDescription(discoveredWinningContext.Record), ex);
                    // Preserve any diagnostics collected before the record-level failure.
                    if (LogCollector.HasLogs())
                    {
                        LogCollector.PrintAll();
                    }

                    Console.WriteLine(
                        $"[Error] [Record] Skipping record {discoveredWinningContext.Record.FormKey}: " +
                        $"{ex.GetType().Name}: {ex.Message}");
                    LogCollector.Clear();
                }
                finally
                {
                    // Covers every early-continue path as well as diagnostics emitted
                    // after the normal phase-specific flush points.
                    if (LogCollector.HasLogs())
                    {
                        LogCollector.PrintAllAndClear();
                    }

                    LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);
                }
            }
            return runDiagnostics.Report;
        }


        private void PreserveBaselineEditorIdIfNeeded(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            RecordPolicySources policySources)
        {
            if (PropertyHandlers.TryGetValue("EditorID", out var handler)
                && policySources.TryGetBaselineEditorId(winningContext.Record, out var editorId)
                && !handler.AreValuesEqual(editorId, handler.GetValue(winningContext.Record)))
            {
                Console.WriteLine("EDID policy: preserving the latest official baseline without loading the merge history");
                CommitOverride(winningContext, state, new Dictionary<string, object?> { ["EditorID"] = editorId });
            }
            PatchDiagnostics.ThrowIfFailed();
        }

        /// <summary>
        /// Gets all record contexts for a given record across the load order.
        /// Each handler must implement this to specify its record types.
        /// The pattern is: cast to TGetter, call ToLink&lt;TGetter&gt;(), then ResolveAllContexts.
        /// GLOB/GMST inherit shared group resolution bounded by the latest subtype transition.
        /// </summary>
        public abstract IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state);

        /// <summary>Stages context insertion and setters before publishing the complete output ancestry.</summary>
        public void CommitOverride(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> sourceContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            Dictionary<string, object?>? propertiesToForward = null)
        {
            PatchDiagnostics.ThrowIfFailed();
            using var diagnostics = new PatchDiagnostics(sourceContext.Record.FormKey);
            RecordOverrideTransaction.Apply(sourceContext, state.PatchMod, candidate =>
            {
                if (propertiesToForward != null) ApplyForwardedProperties(candidate, propertiesToForward);
                diagnostics.Report.ThrowIfFailed();
            });
        }

        /// <summary>
        /// Applies forwarded properties to the record.
        /// Each property is applied through its registered handler, including the composite header flags.
        /// Override this method if you need custom property application logic.
        /// </summary>
        /// <param name="record">The record to apply properties to</param>
        /// <param name="propertiesToForward">Dictionary of properties to forward</param>
        public virtual void ApplyForwardedProperties(IMajorRecord record, Dictionary<string, object?> propertiesToForward)
        {
            foreach (var (propertyName, value) in propertiesToForward)
            {
                if (PropertyHandlers.TryGetValue(propertyName, out var handler))
                {
                    try
                    {
                        if (LogCollector.IsDetailedMode) Console.WriteLine($"[{propertyName}] Applying value: {FormatForLogWithWarning(propertyName, handler, value, "apply", deepDiveRecord: LogCollector.IsDeepDiveMode)}, Type: {value?.GetType()}");
                        using var diagnostics = new PatchDiagnostics(record.FormKey);
                        handler.SetValue(record, value);
                        diagnostics.Report.ThrowIfFailed();
                        // Null setters have record-specific absence/default normalization. Non-null
                        // values must survive writing under the handler's own semantic comparer.
                        if (value != null && !handler.AreValuesEqual(handler.GetValue(record), value))
                        {
                            throw new InvalidOperationException($"Property {propertyName} did not retain the selected value.");
                        }
                        diagnostics.Report.ThrowIfFailed();
                    }
                    catch (Exception ex)
                    {
                        LogCollector.AddError(propertyName, $"Could not apply property to record {record.FormKey}", ex);
                        throw new InvalidOperationException(
                            $"Property {propertyName} was not applied to record {record.FormKey}", ex);
                    }
                }
            }
        }
    }
}
