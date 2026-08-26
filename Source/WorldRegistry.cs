using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace PuppetMaster;

internal class WorldRegistry
{
    public IReadOnlyList<string> Worlds { get; }

    public WorldRegistry(IDataManager dataManager)
    {
        Worlds = dataManager.GetExcelSheet<World>()
                            .Where(w => w.IsPublic && !string.IsNullOrEmpty(w.Name.ExtractText()))
                            .Select(w => w.Name.ExtractText())
                            .OrderBy(name => name)
                            .ToList();
    }
}
