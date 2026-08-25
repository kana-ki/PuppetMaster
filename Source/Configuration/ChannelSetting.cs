namespace PuppetMaster;

public class ChannelSetting
{
    public int ChatType { get; set; }
    public string Name { get; set; } = string.Empty;
    //---- Deprecated, setting will be managed per Reaction
    public bool Enabled { get; set; }
}
