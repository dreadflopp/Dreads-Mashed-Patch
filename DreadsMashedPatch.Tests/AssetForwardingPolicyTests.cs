using System.Text.Json;
using DreadsMashedPatch.AssetForwarding;
using DreadsMashedPatch.Enums;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("AlwaysWinningModTests")]
public sealed partial class AssetForwardingPolicyTests : IDisposable
{
    private static readonly ModKey Original = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey Earlier = ModKey.FromNameAndExtension("AssetEarlier.esp");
    private static readonly ModKey Winner = ModKey.FromNameAndExtension("AssetWinner.esp");
    private static readonly ModKey Patch = ModKey.FromNameAndExtension("AssetPatch.esp");
    private static readonly FormKey Key = new(Original, 0x800);
    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());    [Fact]
    public void FixedForwardingCannotBeDisabledBySavedResourceOrAppearanceFields()
    {
        var configuration = JsonSerializer.Deserialize<PatcherConfiguration>("""
            {"Forwarding":{"Assets":{"Meshes":false,"Textures":false,"Animations":false,
              "Sounds":false,"Music":false,"Scripts":false,"IconsAndImages":false},
              "ForwardAppearanceGroups":false}}
            """)!;
        var copy = JsonSerializer.Deserialize<PatcherConfiguration>(JsonSerializer.Serialize(configuration.Copy()))!;
        PatcherSettings.Apply(copy);
        foreach (var rule in Enum.GetValues<ForwardingRule>().Where(rule => rule != ForwardingRule.Trees))
        {
            Assert.True(configuration.Forwarding.Allows(rule));
            Assert.True(copy.Forwarding.Allows(rule));
            Assert.True(PatcherSettings.AllowsForwarding(rule));
        }
        Assert.False(copy.Forwarding.ForwardTreeGroups);
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(copy));
        var forwarding = json.RootElement.GetProperty("Forwarding");
        Assert.False(forwarding.TryGetProperty("Assets", out _));
        Assert.False(forwarding.TryGetProperty("ForwardAppearanceGroups", out _));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TreeOptionRoundTripsWithoutChangingFixedGroups(bool allow)
    {
        var configuration = new PatcherConfiguration();
        configuration.Forwarding.ForwardTreeGroups = allow;
        var copy = JsonSerializer.Deserialize<PatcherConfiguration>(JsonSerializer.Serialize(configuration))!.Copy();
        PatcherSettings.Apply(copy);
        Assert.Equal(allow, PatcherSettings.AllowsForwarding(ForwardingRule.Trees));
        foreach (var rule in Enum.GetValues<ForwardingRule>().Where(rule => rule != ForwardingRule.Trees))
            Assert.True(PatcherSettings.AllowsForwarding(rule));
    }

    [Theory]
    [InlineData(ForwardingRule.Meshes)]
    [InlineData(ForwardingRule.Textures)]
    [InlineData(ForwardingRule.Appearance)]
    [InlineData(ForwardingRule.Animations)]
    [InlineData(ForwardingRule.Sounds)]
    [InlineData(ForwardingRule.Music)]
    [InlineData(ForwardingRule.Scripts)]
    [InlineData(ForwardingRule.IconsAndImages)]
    public void DefaultAssetFamiliesForwardEarlierGroupChanges(ForwardingRule family)
    {
        var (handler, original, earlier, winner, member) = Versions(family);
        var saved = Assert.Single(Run(handler, new PatcherConfiguration(), original, earlier, winner).EnumerateMajorRecords());
        var property = handler.PropertyHandlers[member];
        Assert.True(property.AreValuesEqual(property.GetValue(earlier), property.GetValue(saved)));
    }

    [Theory]
    [InlineData(ForwardingRule.Sounds)]
    [InlineData(ForwardingRule.Music)]
    [InlineData(ForwardingRule.Textures)]
    public void MetadataOnlyChangesForwardWithTheirGroupWithoutRequiringFilenameChanges(ForwardingRule family)
    {
        var (handler, original, earlier, winner, _) = Versions(family);
        var filenameMembers = family switch
        {
            ForwardingRule.Sounds => new[] { "SoundFiles" },
            ForwardingRule.Music => new[] { "TrackFilename", "FinaleFilename" },
            _ => new[] { "FillTexture" }
        };
        foreach (var key in filenameMembers)
            handler.PropertyHandlers[key].SetValue(earlier, handler.PropertyHandlers[key].GetValue(original));
        if (earlier is SoundDescriptor sound) sound.LoopAndRumble = new SoundLoopAndRumble { Loop = SoundDescriptor.LoopType.Loop };
        var config = new PatcherConfiguration();
        var patch = Run(handler, config, original, earlier, winner);
        var saved = Assert.Single(patch.EnumerateMajorRecords());
        var group = AssetGroupCatalog.For(handler.GetType()).Single(g => g.Members.Contains(filenameMembers[0]));
        foreach (var key in group.Members)
            Assert.True(handler.PropertyHandlers[key].AreValuesEqual(
                handler.PropertyHandlers[key].GetValue(earlier), handler.PropertyHandlers[key].GetValue(saved)), key);
    }

    [Fact]
    public void EveryDeclaredGroupHasRegisteredDisjointMembersAndActivePolicyHooks()
    {
        foreach (var type in typeof(AbstractRecordHandler).Assembly.GetTypes()
                     .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type)))
        {
            var groups = AssetGroupCatalog.For(type);
            if (groups.Count == 0) continue;
            var handler = (AbstractRecordHandler)System.Activator.CreateInstance(type)!;
            var members = new HashSet<string>();
            foreach (var group in groups)
            {
                Assert.NotEmpty(group.Members);
                foreach (var key in group.Members)
                {
                    Assert.True(handler.PropertyHandlers.ContainsKey(key), $"{type.Name}.{group.Name}: missing {key}");
                    Assert.True(members.Add(key), $"{type.Name}: duplicate owner for {key}");
                }
            }
            foreach (var hook in new[] { "InitializePropertyContexts", "CoordinateForwardedProperties" })
                Assert.Equal(type, type.GetMethod(hook, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.DeclaringType);
        }
    }

    public static IEnumerable<object[]> FamilyCases() => Enum.GetValues<ForwardingRule>()
        .SelectMany(family => (family == ForwardingRule.Trees ? new[] { false, true } : new[] { true })
            .Select(allow => new object[] { family, allow }));

    [Fact]
    public void TypedAssetFieldsHaveExplicitOwnersIncludingInheritedAndNestedFields()
    {
        var visited = new HashSet<string>();
        foreach (var type in typeof(AbstractRecordHandler).Assembly.GetTypes()
                     .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type)))
        {
            var name = type.Name.Replace("RecordHandler", "");
            if (name == "DialogResponse") name = "DialogResponses";
            var getter = typeof(INpcGetter).Assembly.GetType($"Mutagen.Bethesda.Skyrim.I{name}Getter");
            if (getter == null) continue;
            var handler = (AbstractRecordHandler)System.Activator.CreateInstance(type)!;
            var roots = AssetGroupCatalog.For(type).SelectMany(group => group.Members).SelectMany(key =>
            {
                var member = handler.PropertyHandlers[key];
                if (member is DreadsMashedPatch.PropertyHandlers.Interfaces.IPropertyGroupHandler grouped)
                    return grouped.GroupedProperties.AsEnumerable();
                return key switch
                {
                    "FlareDefinition" => new[] { "Sprites" },
                    "TextureDefinition" => typeof(DreadsMashedPatch.PropertyHandlers.TextureSet.TextureDefinition).GetProperties().Select(p => p.Name),
                    "MuzzleFlashBehavior" => new[] { "MuzzleFlashModel" },
                    "ModelAndBounds" => new[] { "Model", "ObjectBounds" },
                    "WorldModelAndBounds" => new[] { "WorldModel", "ObjectBounds" },
                    _ => new[] { key }
                };
            }).ToArray();
            foreach (var path in AssetPaths(getter, "", []))
            {
                if (type == typeof(WorldspaceRecordHandler) && path == "CanopyShadow") continue;
                Assert.True(roots.Any(root => path == root || path.StartsWith(root + ".", StringComparison.Ordinal)),
                    $"No asset policy owner: {type.Name}.{path}");
                visited.Add(type.Name + "." + path);
            }
        }
        Assert.NotEmpty(visited);
    }

    private static IEnumerable<string> AssetPaths(Type type, string path, HashSet<Type> ancestors)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (typeof(IAssetLinkGetter).IsAssignableFrom(type)) { yield return path; yield break; }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
        {
            foreach (var item in AssetPaths(type.GenericTypeArguments[0], path, ancestors)) yield return item;
            yield break;
        }
        if (type.IsGenericType && type.Name == "IGenderedItemGetter`1")
        {
            foreach (var side in new[] { "Male", "Female" })
            foreach (var item in AssetPaths(type.GenericTypeArguments[0], path + "." + side, ancestors)) yield return item;
            yield break;
        }
        if (type.Namespace != "Mutagen.Bethesda.Skyrim" || ancestors.Contains(type)
            || (path.Length > 0 && typeof(IMajorRecordGetter).IsAssignableFrom(type))) yield break;
        var next = new HashSet<Type>(ancestors) { type };
        foreach (var property in type.GetInterfaces().Append(type).SelectMany(t => t.GetProperties())
                     .Where(property => property.GetMethod?.IsStatic == false && property.GetIndexParameters().Length == 0)
                     .DistinctBy(property => property.Name))
        {
            if (property.Name is "Persistent" or "Temporary" or "TopCell" or "SubCells" or "Responses") continue;
            foreach (var item in AssetPaths(property.PropertyType, path.Length == 0 ? property.Name : path + "." + property.Name, next))
                yield return item;
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GroupReversionRequiresOwnerPermission(bool authorized)
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Textures);
        var config = new PatcherConfiguration();
        if (authorized) config.CompatibilityRules = [new VirtualMasterRule
            { TargetMod = Winner.FileName.String, VirtualMasters = [Earlier.FileName.String] }];
        var patch = Run(handler, config, original, earlier, winner);
        if (authorized) Assert.Empty(patch.EnumerateMajorRecords());
        else
        {
            var saved = Assert.IsType<EffectShader>(Assert.Single(patch.EnumerateMajorRecords()));
            Assert.Equal("b.dds", saved.FillTexture!.GivenPath);
            Assert.Equal(2, saved.FillTextureScaleU);
        }
    }

    [Theory]
    [MemberData(nameof(FamilyCases))]
    public void FixedGroupsForwardAndTreeOptionControlsItsGroupThroughBinaryOutput(ForwardingRule family, bool allow)
    {
        var (handler, original, earlier, winner, member) = Versions(family);
        var config = new PatcherConfiguration();
        if (family == ForwardingRule.Trees) config.Forwarding.ForwardTreeGroups = allow;
        config.Forwarding.EditorIdPolicy = EditorIdForwardingPolicy.StandardForwarding;
        original.EditorID = winner.EditorID = "Original";
        earlier.EditorID = "Earlier";
        var result = Run(handler, config, original, earlier, winner);
        var saved = Assert.Single(result.EnumerateMajorRecords());
        Assert.Equal("Earlier", saved.EditorID); // unrelated forwarding still works
        var property = handler.PropertyHandlers[member];
        Assert.True(property.AreValuesEqual(property.GetValue(allow ? earlier : winner), property.GetValue(saved)), family.ToString());
        var selectedGroup = AssetGroupCatalog.For(handler.GetType()).Single(group => group.Members.Contains(member));
        foreach (var key in selectedGroup.Members)
        {
            var component = handler.PropertyHandlers[key];
            Assert.True(component.AreValuesEqual(component.GetValue(allow ? earlier : winner), component.GetValue(saved)),
                $"{family}.{selectedGroup.Name}.{key}");
        }
        using var stream = new MemoryStream();
        result.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        Assert.True(property.AreValuesEqual(property.GetValue(saved), property.GetValue(Assert.Single(overlay.EnumerateMajorRecords()))));
        if (saved is Weapon weapon && family == ForwardingRule.Scripts)
            Assert.Equal(((Weapon)winner).VirtualMachineAdapter!.Version, weapon.VirtualMachineAdapter!.Version);
    }

    [Fact]
    public void ShaderMetadataChangeSelectsItsMatchingTexturesInsteadOfCreatingHybrid()
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Textures);
        ((EffectShader)winner).FillTextureScaleU = 3;
        earlier.EditorID = "Earlier";
        var config = new PatcherConfiguration();
        config.Forwarding.EditorIdPolicy = EditorIdForwardingPolicy.StandardForwarding;
        var saved = Assert.IsType<EffectShader>(Assert.Single(Run(handler, config, original, earlier, winner).EnumerateMajorRecords()));
        Assert.Equal("a.dds", saved.FillTexture!.GivenPath);
        Assert.Equal(3, saved.FillTextureScaleU);
    }

    [Fact]
    public void OrdinaryIdleFlagsPreserveWinnerUnknownBitsThroughBinaryReload()
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Animations);
        ((IdleAnimation)earlier).Flags = IdleAnimation.Flag.NoAttacking;
        ((IdleAnimation)winner).Flags = (IdleAnimation.Flag)0x40;
        var config = new PatcherConfiguration();
        var patch = Run(handler, config, original, earlier, winner);
        var saved = Assert.IsType<IdleAnimation>(Assert.Single(patch.EnumerateMajorRecords()));
        Assert.Equal(IdleAnimation.Flag.NoAttacking | (IdleAnimation.Flag)0x40, saved.Flags);
        Assert.Equal("EventB", saved.AnimationEvent);
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        Assert.Equal(saved.Flags, Assert.Single(overlay.IdleAnimations).Flags);
    }

    [Fact]
    public void GroupedAppearancePreservesIndependentProtectionAndHeight()
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Appearance);
        ((Npc)earlier).Height = 1.2f;
        ((Npc)earlier).Configuration.Flags |= NpcConfiguration.Flag.Essential;
        var config = new PatcherConfiguration();
        var saved = Assert.IsType<Npc>(Assert.Single(Run(handler, config, original, earlier, winner).EnumerateMajorRecords()));
        Assert.Equal(1.2f, saved.Height);
        Assert.True(saved.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Essential));
        Assert.Equal(2, saved.FaceMorph!.NoseLongVsShort);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WaterRenderingAndDamageFlagsHaveDisjointOwnershipAndPreserveUnknownBits(bool absent)
    {
        var handler = new WaterRecordHandler();
        var record = new Water(Key, SkyrimRelease.SkyrimSE)
            { Flags = absent ? null : (Water.Flag)0x40 | Water.Flag.CausesDamage };
        handler.PropertyHandlers["Flags.Surface"].SetValue(record, Water.Flag.EnableFlowmap);
        Assert.True(record.Flags!.Value.HasFlag(Water.Flag.EnableFlowmap));
        Assert.Equal(!absent, record.Flags.Value.HasFlag(Water.Flag.CausesDamage));
        if (!absent) Assert.True(record.Flags.Value.HasFlag((Water.Flag)0x40));
        handler.PropertyHandlers["Flags"].SetValue(record, Water.Flag.CausesDamage);
        Assert.True(record.Flags.Value.HasFlag(Water.Flag.EnableFlowmap));
        handler.PropertyHandlers["Flags.Surface"].SetValue(record, (Water.Flag)0);
        Assert.Equal(Water.Flag.CausesDamage, record.Flags.Value & (Water.Flag.CausesDamage | Water.Flag.EnableFlowmap));
        var empty = new Water(new FormKey(Original, 0x801), SkyrimRelease.SkyrimSE) { Flags = null };
        handler.PropertyHandlers["Flags.Surface"].SetValue(empty, (Water.Flag)0);
        Assert.Null(empty.Flags);
    }

    [Fact]
    public void ScopeMaskAndOrdinaryRaceFlagsPreserveUnknownBits()
    {
        var weapon = new Weapon(Key, SkyrimRelease.SkyrimSE)
            { Data = new WeaponData { Flags = WeaponData.Flag.HasScope | (WeaponData.Flag)0x40000000 } };
        var weaponHandler = new WeaponRecordHandler();
        weaponHandler.PropertyHandlers["Data.Flags"].SetValue(weapon, (WeaponData.Flag)1);
        Assert.True(weapon.Data.Flags.HasFlag(WeaponData.Flag.HasScope));
        Assert.True(weapon.Data.Flags.HasFlag((WeaponData.Flag)0x40000000));
        weaponHandler.PropertyHandlers["Data.Flags.HasScope"].SetValue(weapon, (WeaponData.Flag)0);
        Assert.False(weapon.Data.Flags.HasFlag(WeaponData.Flag.HasScope));
        Assert.True(weapon.Data.Flags.HasFlag((WeaponData.Flag)1));

        var race = new Race(Key, SkyrimRelease.SkyrimSE)
            { Flags = Race.Flag.FaceGenHead | (Race.Flag)(1UL << 40) };
        var raceHandler = new RaceRecordHandler();
        raceHandler.PropertyHandlers["Flags"].SetValue(race, Race.Flag.Playable);
        Assert.False(race.Flags.HasFlag(Race.Flag.FaceGenHead));
        Assert.True(race.Flags.HasFlag(Race.Flag.Playable));
        Assert.True(race.Flags.HasFlag((Race.Flag)(1UL << 40)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PrioritySnapshotsAndIgnoredSourcesRetainTheirPrecedence(bool priority)
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Meshes);
        var config = new PatcherConfiguration();
        if (priority) config.AlwaysWinningMods = [Earlier.FileName.String];
        else
        {
            config.IgnoredMods = [Earlier.FileName.String];
        }
        var patch = Run(handler, config, original, earlier, winner);
        if (!priority) Assert.Empty(patch.EnumerateMajorRecords());
        else
        {
            var saved = Assert.IsType<Static>(Assert.Single(patch.EnumerateMajorRecords()));
            Assert.Equal("b.nif", saved.Model!.File.GivenPath);
            Assert.Equal("b_lod.nif", saved.Lod!.Level0.GivenPath);
            Assert.Equal((short)7, saved.ObjectBounds!.First.X);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TreeOptionControlsOnlyItsCompleteGroup(bool trees)
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Trees);
        var config = new PatcherConfiguration();
        config.Forwarding.ForwardTreeGroups = trees;
        var patch = Run(handler, config, original, earlier, winner);
        if (!trees) Assert.Empty(patch.Trees);
        else
        {
            var saved = Assert.Single(patch.Trees);
            Assert.Equal("b.nif", saved.Model!.File.GivenPath);
            Assert.Equal(9, saved.TrunkFlexibility);
            Assert.Equal("EarlierTree", Assert.Single(saved.VirtualMachineAdapter!.Scripts).Name);
            Assert.Equal((int)Tree.MajorFlag.HasDistantLOD, saved.MajorRecordFlagsRaw);
        }
    }

    [Fact]
    public void BookTextAndTranslationsForwardNormally()
    {
        Book Version(string path) => new(Key, SkyrimRelease.SkyrimSE)
        {
            BookText = new Mutagen.Bethesda.Strings.TranslatedString(Mutagen.Bethesda.Strings.Language.English,
                $"<img src='img://Textures/{path}.png'>")
        };
        var original = Version("a");
        var earlier = Version("b");
        earlier.BookText.Set(Mutagen.Bethesda.Strings.Language.French, "French image text");
        var winner = Version("a");
        var config = new PatcherConfiguration();
        var patch = Run(new BookRecordHandler(), config, original, earlier, winner);
        var saved = Assert.IsType<Book>(Assert.Single(patch.EnumerateMajorRecords()));
        Assert.Equal(earlier.BookText.String, saved.BookText.String);
        Assert.True(saved.BookText.TryLookup(Mutagen.Bethesda.Strings.Language.French, out var french));
        Assert.Equal("French image text", french);
    }

    [Fact]
    public void ArmorMeshesAndIconsHaveIndependentOwnersThroughBinaryOutput()
    {
        var original = new Armor(Key, SkyrimRelease.SkyrimSE)
        {
            WorldModel = new GenderedItem<ArmorModel?>(new ArmorModel
            {
                Model = Mesh("a.nif"), Icons = new Icons { LargeIconFilename = new AssetLink<SkyrimTextureAssetType>("a.dds") }
            }, null)
        };
        var earlier = original.DeepCopy();
        earlier.WorldModel!.Male!.Model = Mesh("b.nif");
        earlier.WorldModel.Male.Icons!.LargeIconFilename = new AssetLink<SkyrimTextureAssetType>("b.dds");
        earlier.Armature.Add(new FormLink<IArmorAddonGetter>(new FormKey(Original, 0xA01)));
        earlier.BodyTemplate = new BodyTemplate { ArmorType = ArmorType.HeavyArmor };
        var config = new PatcherConfiguration();
        var patch = Run(new ArmorRecordHandler(), config, original, earlier, original.DeepCopy());
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var saved = Assert.Single(overlay.Armors);
        Assert.Equal("b.nif", saved.WorldModel!.Male!.Model!.File.GivenPath);
        Assert.Equal("b.dds", saved.WorldModel.Male.Icons!.LargeIconFilename!.GivenPath);
        Assert.Single(saved.Armature);
        Assert.Equal(ArmorType.HeavyArmor, saved.BodyTemplate!.ArmorType);
    }

    [Fact]
    public void ArmorAddonGenderMeshesSkinTexturesAndGameplayForwardSeparately()
    {
        var original = new ArmorAddon(Key, SkyrimRelease.SkyrimSE)
        {
            WorldModel = new GenderedItem<Model?>(Mesh("male.nif"), Mesh("female.nif")),
            FirstPersonModel = new GenderedItem<Model?>(Mesh("male1st.nif"), Mesh("female1st.nif"))
        };
        var earlier = original.DeepCopy();
        earlier.WorldModel!.Male = Mesh("male_new.nif");
        earlier.FirstPersonModel!.Male = Mesh("male1st_new.nif");
        earlier.WeightSliderEnabled.Male = true;
        earlier.SkinTexture = new GenderedItem<IFormLinkNullableGetter<ITextureSetGetter>>(new FormLinkNullable<ITextureSetGetter>(new FormKey(Original, 0xA01)), new FormLinkNullable<ITextureSetGetter>());
        earlier.TextureSwapList = new GenderedItem<IFormLinkNullableGetter<IFormListGetter>>(new FormLinkNullable<IFormListGetter>(new FormKey(Original, 0xA02)), new FormLinkNullable<IFormListGetter>());
        earlier.BodyTemplate = new BodyTemplate { ArmorType = ArmorType.HeavyArmor, FirstPersonFlags = BipedObjectFlag.Body };
        earlier.AdditionalRaces.Add(new FormLink<IRaceGetter>(new FormKey(Original, 0xA03)));
        var winner = original.DeepCopy();
        winner.WorldModel!.Female = Mesh("female_new.nif");
        var config = new PatcherConfiguration();
        var saved = Assert.Single(Run(new ArmorAddonRecordHandler(), config, original, earlier, winner).ArmorAddons);
        Assert.Equal("male_new.nif", saved.WorldModel!.Male!.File.GivenPath);
        Assert.Equal("male1st_new.nif", saved.FirstPersonModel!.Male!.File.GivenPath);
        Assert.Equal("female_new.nif", saved.WorldModel.Female!.File.GivenPath);
        Assert.True(saved.WeightSliderEnabled.Male);
        Assert.Equal(earlier.SkinTexture?.Male.FormKey, saved.SkinTexture?.Male.FormKey);
        Assert.Equal(earlier.TextureSwapList?.Male.FormKey, saved.TextureSwapList?.Male.FormKey);
        Assert.Equal(ArmorType.HeavyArmor, saved.BodyTemplate!.ArmorType);
        Assert.Equal(BipedObjectFlag.Body, saved.BodyTemplate.FirstPersonFlags);
        Assert.Single(saved.AdditionalRaces);
    }

    [Fact]
    public void HeadPartMeshGroupsAndEligibilityForwardNormally()
    {
        var original = new HeadPart(Key, SkyrimRelease.SkyrimSE) { Model = Mesh("a.nif") };
        var earlier = original.DeepCopy();
        earlier.Model = Mesh("b.nif");
        earlier.Flags = HeadPart.Flag.Male | HeadPart.Flag.UseSolidTint;
        earlier.ValidRaces.SetTo(new FormKey(Original, 0xA01));
        var config = new PatcherConfiguration();
        var saved = Assert.Single(Run(new HeadPartRecordHandler(), config, original, earlier, original.DeepCopy()).HeadParts);
        Assert.Equal("b.nif", saved.Model!.File.GivenPath);
        Assert.True(saved.Flags.HasFlag(HeadPart.Flag.UseSolidTint));
        Assert.True(saved.Flags.HasFlag(HeadPart.Flag.Male));
        Assert.Equal(earlier.ValidRaces.FormKey, saved.ValidRaces.FormKey);
    }

    [Fact]
    public void EyeTextureForwardsAndReadsBinaryOverlays()
    {
        var original = new Eyes(Key, SkyrimRelease.SkyrimSE) { Icon = new AssetLink<SkyrimTextureAssetType>("a.dds") };
        var earlier = original.DeepCopy();
        earlier.Icon = new AssetLink<SkyrimTextureAssetType>("b.dds");
        earlier.Flags = Eyes.Flag.Playable;
        var config = new PatcherConfiguration();
        var patch = Run(new EyesRecordHandler(), config, original, earlier, original.DeepCopy());
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var saved = Assert.Single(overlay.Eyes);
        var handler = new DreadsMashedPatch.PropertyHandlers.Eyes.IconHandler();
        Assert.True(handler.AreValuesEqual(handler.GetValue(earlier), handler.GetValue(saved)));
    }

    [Fact]
    public void RaceHeadShapeAndResourceGroupsForwardAlongsideOrdinaryChoices()
    {
        var original = new Race(Key, SkyrimRelease.SkyrimSE)
        {
            HeadData = new GenderedItem<HeadData?>(new HeadData { Model = Mesh("a.nif") }, null),
            NumberOfTintsInList = 0
        };
        var earlier = original.DeepCopy();
        earlier.HeadData!.Male!.Model = Mesh("b.nif");
        earlier.HeadData.Male.HeadParts.Add(new HeadPartReference { Number = 2 });
        earlier.FacegenMainClamp = 2;
        earlier.HeadData.Male.DefaultFaceTexture.SetTo(new FormKey(Original, 0xA01));
        earlier.HeadData.Male.TintMasks.Add(new TintAssets { Index = 7 });
        earlier.NumberOfTintsInList = 1;
        earlier.HeadData.Male.AvailableHairColors.Add(new FormLink<IColorRecordGetter>(new FormKey(Original, 0xA02)));
        earlier.HeadData.Male.RacePresets.Add(new FormLink<INpcGetter>(new FormKey(Original, 0xA03)));
        earlier.Flags = Race.Flag.FaceGenHead;
        earlier.Height.Male = 2;
        var config = new PatcherConfiguration();
        var patch = Run(new RaceRecordHandler(), config, original, earlier, original.DeepCopy());
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var saved = Assert.Single(overlay.Races);
        Assert.Equal("b.nif", saved.HeadData!.Male!.Model!.File.GivenPath);
        Assert.Single(saved.HeadData.Male.HeadParts);
        Assert.Equal(2, saved.FacegenMainClamp);
        Assert.Single(saved.HeadData.Male.TintMasks);
        Assert.Equal((ushort?)1, saved.NumberOfTintsInList);
        Assert.False(saved.HeadData.Male.DefaultFaceTexture.IsNull);
        Assert.Single(saved.HeadData.Male.AvailableHairColors);
        Assert.Single(saved.HeadData.Male.RacePresets);
        Assert.Null(saved.HeadData.Female);
        Assert.True(saved.Flags.HasFlag(Race.Flag.FaceGenHead));
        Assert.Equal(2, saved.Height.Male);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RaceAbsentHeadDecisionSuppressesSelectedChildWritesInEitherOrder(bool reverse)
    {
        var handler = new RaceRecordHandler();
        var source = new Race(Key, SkyrimRelease.SkyrimSE)
        {
            HeadData = new GenderedItem<HeadData?>(new HeadData { Model = Mesh("a.nif") }, new HeadData())
        };
        var values = handler.PropertyHandlers.Keys.Where(key => key.StartsWith("HeadData.") && key != "HeadData.Presence")
            .ToDictionary(key => key, key => handler.PropertyHandlers[key].GetValue(source));
        values["HeadData.Presence"] = new DreadsMashedPatch.PropertyHandlers.Race.RaceHeadDataPresence(false, false, false);
        if (reverse) values = values.Reverse().ToDictionary(pair => pair.Key, pair => pair.Value);
        handler.ApplyForwardedProperties(source, values);
        Assert.Null(source.HeadData);
    }

    [Theory]
    [InlineData("Speed")]
    [InlineData("Damage")]
    [InlineData("AttackRace")]
    [InlineData("MovementNames")]
    public void RaceGameplayEditsCannotTakeOwnershipOfRigReferences(string field)
    {
        var original = RigRace();
        var earlier = original.DeepCopy();
        earlier.SkeletalModel!.Male = Skeleton("b.nif");
        earlier.BehaviorGraph.Male = Behavior("b.hkx");
        var winner = original.DeepCopy();
        switch (field)
        {
            case "Speed": winner.MovementTypes[0].Overrides!.ForwardWalk = 2; break;
            case "Damage": winner.Attacks[0].AttackData!.DamageMult = 2; break;
            case "AttackRace": winner.AttackRace.SetTo(new FormKey(Original, 0xA01)); break;
            case "MovementNames": winner.MovementTypeNames.Add("RUN1"); break;
        }
        var saved = Assert.Single(Run(new RaceRecordHandler(), new PatcherConfiguration(), original, earlier, winner).Races);
        Assert.Equal("b.nif", saved.SkeletalModel!.Male!.File.GivenPath);
        Assert.Equal("b.hkx", saved.BehaviorGraph.Male!.File.GivenPath);
        var property = new RaceRecordHandler().PropertyHandlers[field switch
        {
            "Speed" => "MovementTypes", "Damage" => "Attacks", "MovementNames" => "MovementTypeNames", _ => "AttackRace"
        }];
        Assert.True(property.AreValuesEqual(property.GetValue(winner), property.GetValue(saved)));
    }

    [Fact]
    public void RaceGameplayAndRigGroupsForwardNormally()
    {
        var original = RigRace();
        var earlier = original.DeepCopy();
        earlier.SkeletalModel!.Male = Skeleton("b.nif");
        earlier.BehaviorGraph.Male = Behavior("b.hkx");
        earlier.BodyPartData.SetTo(new FormKey(Original, 0xA01));
        earlier.AttackRace.SetTo(new FormKey(Original, 0xA02));
        earlier.Attacks[0].AttackData!.DamageMult = 2;
        earlier.MovementTypes[0].Overrides!.ForwardWalk = 2;
        earlier.MovementTypeNames.Add("RUN1");
        var config = new PatcherConfiguration();
        var saved = Assert.Single(Run(new RaceRecordHandler(), config, original, earlier, original.DeepCopy()).Races);
        Assert.Equal("b.nif", saved.SkeletalModel!.Male!.File.GivenPath);
        Assert.Equal("b.hkx", saved.BehaviorGraph.Male!.File.GivenPath);
        Assert.Equal(earlier.BodyPartData.FormKey, saved.BodyPartData.FormKey);
        Assert.Equal(earlier.AttackRace.FormKey, saved.AttackRace.FormKey);
        Assert.Equal(2, saved.Attacks[0].AttackData!.DamageMult);
        Assert.Equal(2, saved.MovementTypes[0].Overrides!.ForwardWalk);
        Assert.Contains("RUN1", saved.MovementTypeNames);
    }

    [Fact]
    public void RaceGenderRigPairsSelectIndependentlyThroughBinaryOutput()
    {
        var original = RigRace();
        var earlier = original.DeepCopy();
        earlier.SkeletalModel!.Male = Skeleton("male_b.nif");
        earlier.BehaviorGraph.Male = Behavior("male_b.hkx");
        var winner = original.DeepCopy();
        winner.SkeletalModel!.Female = Skeleton("female_c.nif");
        winner.BehaviorGraph.Female = Behavior("female_c.hkx");
        var config = new PatcherConfiguration();
        config.Forwarding.EditorIdPolicy = EditorIdForwardingPolicy.StandardForwarding;
        earlier.EditorID = "Earlier";
        var patch = Run(new RaceRecordHandler(), config, original, earlier, winner);
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var saved = Assert.Single(overlay.Races);
        Assert.Equal("male_b.nif", saved.SkeletalModel!.Male!.File.GivenPath);
        Assert.Equal("male_b.hkx", saved.BehaviorGraph.Male!.File.GivenPath);
        Assert.Equal("female_c.nif", saved.SkeletalModel.Female!.File.GivenPath);
        Assert.Equal("female_c.hkx", saved.BehaviorGraph.Female!.File.GivenPath);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RaceRigOverlayCopiesAreDetachedAndCanClearOneGender(bool female)
    {
        var source = new SkyrimMod(Original, SkyrimRelease.SkyrimSE);
        source.Races.Add(RigRace());
        source.Races.Single().BehaviorGraph.Male!.AlternateTextures =
            [new AlternateTexture { Name = "Node", Index = 1 }];
        using var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Original);
        var handler = new RaceRecordHandler();
        var target = RigRace();
        var gender = female ? "Female" : "Male";
        var keys = new[] { $"SkeletalModel.{gender}", $"BehaviorGraph.{gender}" };
        foreach (var key in keys) handler.PropertyHandlers[key].SetValue(target, handler.PropertyHandlers[key].GetValue(overlay.Races.Single()));
        if (!female)
        {
            target.BehaviorGraph.Male!.AlternateTextures![0].Index = 5;
            Assert.Equal(1, overlay.Races.Single().BehaviorGraph.Male!.AlternateTextures![0].Index);
        }
        var other = female ? MaleFemaleGender.Male : MaleFemaleGender.Female;
        foreach (var key in keys) handler.PropertyHandlers[key].SetValue(target, null);
        var cleared = female ? MaleFemaleGender.Female : MaleFemaleGender.Male;
        Assert.Null(target.SkeletalModel![cleared]);
        Assert.Null(target.BehaviorGraph[cleared]);
        Assert.Equal("a.nif", target.SkeletalModel[other]!.File.GivenPath);
        Assert.Equal("a.hkx", target.BehaviorGraph[other]!.File.GivenPath);
    }

    [Theory]
    [InlineData("LoopingSecondsMin", 2)]
    [InlineData("LoopingSecondsMax", 3)]
    [InlineData("Flags", 2)]
    [InlineData("AnimationGroupSection", 4)]
    [InlineData("ReplayDelay", 5)]
    public void IdleLegacyDataCannotSelectTheFilenameAndEvent(string field, int value)
    {
        var (handler, original, earlier, winner, _) = Versions(ForwardingRule.Animations);
        var idle = (IdleAnimation)winner;
        switch (field)
        {
            case "LoopingSecondsMin": idle.LoopingSecondsMin = (byte)value; break;
            case "LoopingSecondsMax": idle.LoopingSecondsMax = (byte)value; break;
            case "Flags": idle.Flags = (IdleAnimation.Flag)value; break;
            case "AnimationGroupSection": idle.AnimationGroupSection = (byte)value; break;
            case "ReplayDelay": idle.ReplayDelay = (ushort)value; break;
        }
        var saved = Assert.Single(Run(handler, new PatcherConfiguration(), original, earlier, winner).IdleAnimations);
        Assert.Equal("b.hkx", saved.Filename!.GivenPath);
        Assert.Equal("EventB", saved.AnimationEvent);
        Assert.True(handler.PropertyHandlers[field].AreValuesEqual(
            handler.PropertyHandlers[field].GetValue(winner), handler.PropertyHandlers[field].GetValue(saved)));
    }

    [Fact]
    public void IdleMarkerAnimationPackageIsSeparateFromItsMarkerMesh()
    {
        var original = new IdleMarker(Key, SkyrimRelease.SkyrimSE) { Model = Mesh("a.nif"), IdleTimer = 1 };
        original.Animations = [new FormLink<IIdleAnimationGetter>(new FormKey(Original, 0xA01))];
        var earlier = original.DeepCopy();
        earlier.Model = Mesh("b.nif");
        earlier.Animations = [new FormLink<IIdleAnimationGetter>(new FormKey(Original, 0xA03)), new FormLink<IIdleAnimationGetter>(new FormKey(Original, 0xA02))];
        earlier.Flags = IdleMarker.Flag.RunInSequence;
        earlier.IdleTimer = 7;
        var winner = original.DeepCopy();
        winner.Flags = (IdleMarker.Flag)0x80;
        var config = new PatcherConfiguration();
        var patch = Run(new IdleMarkerRecordHandler(), config, original, earlier, winner);
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var saved = Assert.Single(overlay.IdleMarkers);
        Assert.Equal("b.nif", saved.Model!.File.GivenPath);
        Assert.Equal(earlier.Animations!.Select(link => link.FormKey), saved.Animations!.Select(link => link.FormKey));
        Assert.Equal(7, saved.IdleTimer);
        Assert.Equal((IdleMarker.Flag)0x80 | (IdleMarker.Flag.RunInSequence), saved.Flags);
    }

    [Fact]
    public void PackageIdleSelectionForwardsWithItsSettings()
    {
        var original = new Package(Key, SkyrimRelease.SkyrimSE)
        {
            IdleAnimations = new PackageIdles { Type = PackageIdles.Types.Random, TimerSetting = 1 }
        };
        original.IdleAnimations.Animations.Add(new FormLink<IIdleAnimationGetter>(new FormKey(Original, 0xA01)));
        var earlier = original.DeepCopy();
        earlier.EditorID = "Earlier";
        earlier.IdleAnimations!.Type = PackageIdles.Types.RunInSequence;
        earlier.IdleAnimations.TimerSetting = 7;
        earlier.IdleAnimations.Animations[0] = new FormLink<IIdleAnimationGetter>(new FormKey(Original, 0xA03));
        earlier.IdleAnimations.Animations.Add(new FormLink<IIdleAnimationGetter>(new FormKey(Original, 0xA02)));
        var config = new PatcherConfiguration();
        config.Forwarding.EditorIdPolicy = EditorIdForwardingPolicy.StandardForwarding;
        var patch = Run(new PackageRecordHandler(), config, original, earlier, original.DeepCopy());
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var saved = Assert.Single(overlay.Packages);
        Assert.True(PackageIdlesMixIn.Equals(earlier.IdleAnimations!, saved.IdleAnimations!));
    }

    private static Race RigRace()
    {
        var race = new Race(Key, SkyrimRelease.SkyrimSE)
        {
            SkeletalModel = new GenderedItem<SimpleModel?>(Skeleton("a.nif"), Skeleton("a.nif")),
            BehaviorGraph = new GenderedItem<ModelBehavior?>(Behavior("a.hkx"), Behavior("a.hkx"))
        };
        var movement = new RaceMovementType { Overrides = new SpeedOverrides { ForwardWalk = 1 } };
        movement.MovementType.SetTo(new FormKey(Original, 0xA10));
        race.MovementTypes.Add(movement);
        race.MovementTypeNames.Add("WALK");
        race.Attacks.Add(new Attack { AttackEvent = "attackStart", AttackData = new AttackData { DamageMult = 1 } });
        return race;
    }

    private static SimpleModel Skeleton(string path) => new() { File = new AssetLink<SkyrimModelAssetType>(path) };
    private static ModelBehavior Behavior(string path) => new() { File = new AssetLink<SkyrimBehaviorAssetType>(path) };
    private static Model Mesh(string path) => new() { File = new AssetLink<SkyrimModelAssetType>(path) };

    private static (AbstractRecordHandler Handler, IMajorRecord Original, IMajorRecord Earlier, IMajorRecord Winner, string Member) Versions(ForwardingRule family)
    {
        IMajorRecord Version(bool changed, bool winning) => family switch
        {
            ForwardingRule.Meshes => new Static(Key, SkyrimRelease.SkyrimSE)
            {
                Model = new Model { File = new AssetLink<SkyrimModelAssetType>(changed ? "b.nif" : "a.nif") },
                ObjectBounds = new ObjectBounds { First = new(winning ? (short)9 : changed ? (short)7 : (short)1, 0, 0) },
                Lod = new Lod { Level0 = new AssetLink<SkyrimModelAssetType>(changed ? "b_lod.nif" : "a_lod.nif") }
            },
            ForwardingRule.Textures => new EffectShader(Key, SkyrimRelease.SkyrimSE)
            {
                FillTexture = new AssetLink<SkyrimTextureAssetType>(changed ? "b.dds" : "a.dds"),
                FillTextureScaleU = changed ? 2 : 1
            },
            ForwardingRule.Appearance => new Npc(Key, SkyrimRelease.SkyrimSE)
                { FaceMorph = new NpcFaceMorph { NoseLongVsShort = changed ? 2 : 0 } },
            ForwardingRule.Animations => new IdleAnimation(Key, SkyrimRelease.SkyrimSE)
            {
                Filename = new AssetLink<SkyrimBehaviorAssetType>(changed ? "b.hkx" : "a.hkx"),
                AnimationEvent = changed ? "EventB" : "EventA"
            },
            ForwardingRule.Sounds => Sound(changed),
            ForwardingRule.Music => new MusicTrack(Key, SkyrimRelease.SkyrimSE)
            {
                TrackFilename = new AssetLink<SkyrimMusicAssetType>(changed ? "b.xwm" : "a.xwm"),
                FinaleFilename = new AssetLink<SkyrimMusicAssetType>(changed ? "b_finale.xwm" : "a_finale.xwm"),
                Duration = changed ? 200 : 100
            },
            ForwardingRule.Scripts => ScriptWeapon(changed, winning),
            ForwardingRule.Trees => TreeVersion(changed),
            ForwardingRule.IconsAndImages => new Ingestible(Key, SkyrimRelease.SkyrimSE)
                { Icons = new Icons { LargeIconFilename = new AssetLink<SkyrimTextureAssetType>(changed ? "b.dds" : "a.dds") } },
            _ => throw new ArgumentOutOfRangeException(nameof(family))
        };
        (AbstractRecordHandler Handler, string Key) pair = family switch
        {
            ForwardingRule.Meshes => (new StaticRecordHandler(), "ModelAndBounds"),
            ForwardingRule.Textures => (new EffectShaderRecordHandler(), "FillTexture"),
            ForwardingRule.Appearance => (new NpcRecordHandler(), "Appearance"),
            ForwardingRule.Animations => (new IdleAnimationRecordHandler(), "Filename"),
            ForwardingRule.Sounds => (new SoundDescriptorRecordHandler(), "SoundFiles"),
            ForwardingRule.Music => (new MusicTrackRecordHandler(), "TrackFilename"),
            ForwardingRule.Scripts => (new WeaponRecordHandler(), "VirtualMachineAdapter"),
            ForwardingRule.Trees => (new TreeRecordHandler(), "ModelAndBounds"),
            ForwardingRule.IconsAndImages => (new IngestibleRecordHandler(), "Icons"),
            _ => throw new ArgumentOutOfRangeException(nameof(family))
        };
        return (pair.Handler, Version(false, false), Version(true, false), Version(false, true), pair.Key);
    }

    private static Tree TreeVersion(bool changed)
    {
        var tree = new Tree(Key, SkyrimRelease.SkyrimSE)
        {
            Model = Mesh(changed ? "b.nif" : "a.nif"),
            TrunkFlexibility = changed ? 9 : 1,
            MajorRecordFlagsRaw = changed ? (int)Tree.MajorFlag.HasDistantLOD : 0,
            VirtualMachineAdapter = new VirtualMachineAdapter()
        };
        if (changed) tree.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "EarlierTree" });
        return tree;
    }

    private static SoundDescriptor Sound(bool changed)
    {
        var sound = new SoundDescriptor(Key, SkyrimRelease.SkyrimSE);
        sound.SoundFiles.Add(new AssetLink<SkyrimSoundAssetType>(changed ? "b.wav" : "a.wav"));
        return sound;
    }
    private static Weapon ScriptWeapon(bool changed, bool winning)
    {
        var weapon = new Weapon(Key, SkyrimRelease.SkyrimSE)
            { VirtualMachineAdapter = new VirtualMachineAdapter { Version = winning ? (short)4 : (short)5 } };
        if (changed) weapon.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "CustomScript" });
        return weapon;
    }

    private static SkyrimMod Run(AbstractRecordHandler handler, PatcherConfiguration configuration, params IMajorRecord[] records)
    {
        PatcherSettings.Apply(configuration);
        var keys = new[] { Original, Earlier, Winner };
        var mods = records.Select((record, i) =>
        {
            var mod = new SkyrimMod(keys[i], SkyrimRelease.SkyrimSE);
            if (i != 0) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = Original });
            var groupProperty = typeof(SkyrimMod).GetProperties().Single(property => property.PropertyType.IsGenericType
                && property.PropertyType.GetGenericTypeDefinition() == typeof(SkyrimGroup<>)
                && property.PropertyType.GenericTypeArguments[0] == record.GetType());
            var group = groupProperty.GetValue(mod)!;
            group.GetType().GetMethod("Add", [record.GetType()])!.Invoke(group, [record]);
            return mod;
        }).ToArray();
        var patch = new SkyrimMod(Patch, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
#pragma warning disable CS0618
        using var state = new SynthesisState<ISkyrimMod, ISkyrimModGetter>(new RunSynthesisMutagenPatcher
        {
            OutputPath = Path.Combine(Path.GetTempPath(), "AssetPatch.esp"), DataFolderPath = Path.GetTempPath(),
            LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "unused-plugins.txt"), GameRelease = GameRelease.SkyrimSE
        }, listings.Select(mod => new LoadOrderListing(mod.ModKey, mod.Enabled)).ToArray(), order,
            order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), null!, patch, null, null, null, CancellationToken.None, null);
#pragma warning restore CS0618
        var contexts = order.PriorityOrder.WinningContextOverrides<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(state.LinkCache).ToArray();
        var report = handler.Process(state, contexts);
        Assert.True(report.Succeeded);
        Assert.Empty(report.SkippedRecords);
        return patch;
    }
}
