namespace DreadsMashedPatch.App.ViewModels;

public sealed class VirtualMasterRuleViewModel : BindableBase
{
    private string _targetMod = string.Empty;
    private string _virtualMastersText = string.Empty;

    public VirtualMasterRuleViewModel(VirtualMasterRule rule)
    {
        _targetMod = rule.TargetMod;
        _virtualMastersText = string.Join(
            Environment.NewLine,
            rule.VirtualMasters.Order(StringComparer.OrdinalIgnoreCase));
    }

    public string TargetMod
    {
        get => _targetMod;
        set
        {
            if (SetProperty(ref _targetMod, value ?? string.Empty))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public string VirtualMastersText
    {
        get => _virtualMastersText;
        set => SetProperty(ref _virtualMastersText, value ?? string.Empty);
    }

    public string DisplayName => string.IsNullOrWhiteSpace(TargetMod)
        ? "New master rule"
        : TargetMod.Trim();

    public VirtualMasterRule ToModel() => new()
    {
        TargetMod = TargetMod,
        VirtualMasters = VirtualMastersText
            .Split(['\r', '\n', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
    };

    public void MergeVirtualMastersFrom(VirtualMasterRuleViewModel other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var virtualMasters = ToModel().VirtualMasters;
        virtualMasters.UnionWith(other.ToModel().VirtualMasters);
        VirtualMastersText = string.Join(
            Environment.NewLine,
            virtualMasters.Order(StringComparer.OrdinalIgnoreCase));
    }
}
