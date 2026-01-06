using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FixLoadMotd;

public class Plugin : BasePlugin
{
    public override string ModuleName => "FixLoadMotd";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "xstage";

    private INetworkStringTableContainer? _networkStringTableContainer;

    public static readonly int LINUX_OFFSET_PREDICT = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 1 : 0;

    private const string INTERFACE_NAME = "Source2EngineToServerStringTable001";
    private const string TABLE_NAME = "InfoPanel";
    private const string STRING_KEY_NAME = "motd";

    public FakeConVar<string> MotdUrl = new("motd_url", "URL to display", "", ConVarFlags.FCVAR_NONE);

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(OnMapStart);

        MotdUrl.ValueChanged += (sender, newUrl) =>
        {
            if (!string.IsNullOrEmpty(newUrl))
            {
                Logger.LogInformation("MOTD URL changed to: {url}", newUrl);
                Server.NextFrame(() => ApplyMotdUrl(newUrl));
            }
        };
    }

    private void OnMapStart(string mapName)
    {
        string url = MotdUrl.Value;

        if (string.IsNullOrEmpty(url))
        {
            var motdPath = Path.Combine(Server.GameDirectory, "csgo", ConVar.Find("motdfile")?.StringValue ?? "motd.txt");
            if (!File.Exists(motdPath))
            {
                Logger.LogWarning("MOTD file not found at {motdPath}. Use 'motd_url <url>' to set the MOTD URL.", motdPath);
                return;
            }

            var readFile = File.ReadAllTextAsync(motdPath);
            readFile.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Logger.LogError("Failed to read motd.txt at {motdPath}", motdPath);
                    return;
                }

                string fileUrl = t.Result.Trim();
                if (string.IsNullOrEmpty(fileUrl))
                {
                    Logger.LogWarning("MOTD file is empty at {motdPath}", motdPath);
                    return;
                }

                Server.NextFrame(() => ApplyMotdUrl(fileUrl));
            });
        }
        else
        {
            Server.NextFrame(() => ApplyMotdUrl(url));
        }
    }

    private void ApplyMotdUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            Logger.LogWarning("Invalid URL format: {url}. Must be a valid HTTP or HTTPS URL.", url);
            return;
        }

        _networkStringTableContainer ??= new(NativeAPI.GetValveInterface(0, INTERFACE_NAME));
        if (_networkStringTableContainer == null)
        {
            Logger.LogError("Failed to create network string table container {interfaceName}", INTERFACE_NAME);
            return;
        }

        INetworkStringTable? table = _networkStringTableContainer.FindTable(TABLE_NAME);
        if (table == null)
        {
            Logger.LogError("Failed to find table {tableName}", TABLE_NAME);
            return;
        }

        SetMOTDValue(table, url);
    }

    private unsafe void SetMOTDValue(INetworkStringTable table, string value)
    {
        var msg = Encoding.UTF8.GetBytes(value + "\0");
        fixed (byte* pMsg = msg)
        {
            SetStringUserDataRequest_t data;

            data.m_pRawData = pMsg;
            data.m_cbDataSize = msg.Length;

            if (table.AddString(true, STRING_KEY_NAME, ref data) != INetworkStringTable.INVALID_STRING_INDEX)
            {
                Logger.LogInformation("Successfully added MOTD string: {url}", value);
                return;
            }
            else
            {
                Logger.LogError("Failed to add MOTD string to table");
            }
        }
    }
}