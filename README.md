# FixLoadMotd
Fix load MOTD & server website button in tab using a configurable ConVar or motd.txt file
<p align="center">
  <img
    src="https://github.com/user-attachments/assets/cb4b9571-07a2-4bb4-bd5f-0c4c0cbdf842"
    alt="Preview"
    width="334"
    height="157"
  />
</p>

## 🚀 Installation
1. Install [CounterStrike Sharp](https://github.com/roflmuffin/CounterStrikeSharp) and [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)
2. Download [FixLoadMotd.zip](https://github.com/wiruwiru/FixLoadMotd/releases/latest) from releases
3. Extract and upload to your game server: `game/csgo/addons/counterstrikesharp/plugins/FixLoadMotd/`

## ⚙️ Configuration
You have two options to configure the MOTD URL:

### Option 1: Using ConVar (Recommended)
Add the ConVar to your server configuration file (e.g., `cfg/server.cfg`):
```
motd_url "https://your-website.com/"
```

### Option 2: Using motd.txt file
Create or edit the `motd.txt` file in your `game/csgo/` directory:
```
https://your-website.com/
```

**Note:** The ConVar takes priority over the motd.txt file. If `motd_url` is set, the plugin will use it instead of reading from motd.txt.
