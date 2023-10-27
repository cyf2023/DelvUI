using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Logging;
using DelvUI.Config.Attributes;
using DelvUI.Config.Tree;
using DelvUI.Helpers;
using DelvUI.Interface;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace DelvUI.Config.Profiles
{
    public class ProfilesManager
    {
        #region Singleton
        public readonly SectionNode ProfilesNode;

        private ProfilesManager()
        {
            // fake nodes
            ProfilesNode = new SectionNode();
            ProfilesNode.Name = "配置文件";

            NestedSubSectionNode subSectionNode = new NestedSubSectionNode();
            subSectionNode.Name = "通用";
            subSectionNode.Depth = 0;

            ProfilesConfigPageNode configPageNode = new ProfilesConfigPageNode();

            subSectionNode.Add(configPageNode);
            ProfilesNode.Add(subSectionNode);

            ConfigurationManager.Instance.AddExtraSectionNode(ProfilesNode);

            // default profile
            if (!Profiles.ContainsKey(DefaultProfileName))
            {
                Profile defaultProfile = new Profile(DefaultProfileName);
                Profiles.Add(DefaultProfileName, defaultProfile);
            }
        }

        private bool ResetProfileToDefault(string profileName, bool forced = false)
        {
            string endPath = Path.Combine(ProfilesPath, profileName + ".delvui");

            if (forced)
            {
                try
                {
                    File.Delete(endPath);
                } catch { }
            }

            if (forced || !File.Exists(endPath))
            {
                try
                {
                    Directory.CreateDirectory(ProfilesPath);
                    File.Copy(MediaDefaultProfilePath, endPath);

                    return true;
                }
                catch (Exception e)
                {
                    PluginLog.Error("复制默认配置文件时出错！：" + e.Message);
                }
            }

            return false;
        }

        public static void Initialize()
        {
            bool attemptRepair = false;

            try
            {
                string jsonString = File.ReadAllText(JsonPath);
                ProfilesManager? instance = JsonConvert.DeserializeObject<ProfilesManager>(jsonString);
                if (instance != null)
                {
                    Instance = instance;

                    bool needsSave = false;
                    foreach (Profile profile in Instance.Profiles.Values)
                    {
                        needsSave |= profile.AutoSwitchData.ValidateRolesData();
                    }

                    if (needsSave)
                    {
                        Instance.Save();
                    }
                }
            }
            catch
            {
                Instance = new ProfilesManager();
                attemptRepair = true;
            }

            if (Instance == null)
            {
                PluginLog.Error("初始化DelvUI配置文件时出错！！！");
                return;
            }

            // attempt to reconstruct profile from files if the Profiles directory is missing
            if (attemptRepair && 
                !ConfigurationManager.Instance.IsFreshInstall() && 
                !Directory.Exists(ProfilesPath))
            {
                Instance.CurrentProfileName = "恢复配置文件";

                Profile defaultProfile = new Profile(Instance.CurrentProfileName);
                Instance.Profiles.Add(Instance.CurrentProfileName, defaultProfile);
            }

            // always make sure the default profile file is present
            if (!File.Exists(DefaultProfilePath))
            {
                if (Instance.ResetProfileToDefault(DefaultProfileName))
                {
                    if (Instance.CurrentProfileName == DefaultProfileName)
                    {
                        Instance.ReloadCurrentProfile();
                    }
                }
            }

            Instance.UpdateSelectedIndex();
            Instance.InitializeDefaultImportData();
        }

        private void InitializeDefaultImportData()
        {
            string importString = File.ReadAllText(MediaDefaultProfilePath);

            string[] importStrings = importString.Trim().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (importStrings.Length == 0)
            {
                return;
            }

            foreach (var str in importStrings)
            {
                try
                {
                    ImportData importData = new ImportData(str);
                    _defaultImportData.Add(importData);
                }
                catch { }
            }
        }

        public static ProfilesManager Instance { get; private set; } = null!;

        ~ProfilesManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Instance = null!;
        }
        #endregion

        private string _currentProfileName = "Default";
        public string CurrentProfileName
        {
            get => _currentProfileName;
            set
            {
                if (_currentProfileName == value)
                {
                    return;
                }

                _currentProfileName = value;

                if (_currentProfileName == null || _currentProfileName.Length == 0)
                {
                    _currentProfileName = DefaultProfileName;
                }

                UpdateSelectedIndex();
            }
        }

        [JsonIgnore] private static string ProfilesPath => Path.Combine(ConfigurationManager.Instance.ConfigDirectory, "Profiles");
        [JsonIgnore] private static string JsonPath => Path.Combine(ProfilesPath, "Profiles.json");
        [JsonIgnore] private static string MediaDefaultProfilePath => Path.Combine(Plugin.AssemblyLocation, "Media", "Profiles", DefaultProfileName + ".delvui");

        [JsonIgnore] private static string DefaultProfileName = "Default";
        [JsonIgnore] private static string DefaultProfilePath = Path.Combine(ProfilesPath, DefaultProfileName + ".delvui");

        [JsonIgnore] private List<ImportData> _defaultImportData = new List<ImportData>();

        [JsonIgnore] private string _newProfileName = "";
        [JsonIgnore] private int _copyFromIndex = 0;
        [JsonIgnore] private int _selectedProfileIndex = 0;
        [JsonIgnore] private string? _errorMessage = null;
        [JsonIgnore] private string? _deletingProfileName = null;
        [JsonIgnore] private string? _resetingProfileName = null;
        [JsonIgnore] private string? _renamingProfileName = null;

        [JsonIgnore] private FileDialogManager _fileDialogManager = new FileDialogManager();

        public SortedList<string, Profile> Profiles = new SortedList<string, Profile>();

        public ImportData? DefaultImportData(Type type)
        {
            return _defaultImportData.FirstOrDefault(o => o.ConfigType == type);
        }

        public Profile CurrentProfile()
        {
            if (_currentProfileName == null || _currentProfileName.Length == 0)
            {
                _currentProfileName = DefaultProfileName;
            }

            return Profiles[_currentProfileName];
        }

        public void SaveCurrentProfile()
        {
            if (ConfigurationManager.Instance == null)
            {
                return;
            }

            try
            {
                Save();
                SaveCurrentProfile(ConfigurationManager.Instance.ExportCurrentConfigs());
            }
            catch (Exception e)
            {
                PluginLog.Error("保存配置文件错误：" + e.Message);
            }
        }

        public void SaveCurrentProfile(string? exportString)
        {
            if (exportString == null)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(ProfilesPath);

                File.WriteAllText(CurrentProfilePath(), exportString);
            }
            catch (Exception e)
            {
                PluginLog.Error("保存配置文件错误：" + e.Message);
            }
        }

        public bool LoadCurrentProfile(string oldProfile)
        {
            try
            {
                string importString = File.ReadAllText(CurrentProfilePath());
                return ConfigurationManager.Instance.ImportProfile(oldProfile, _currentProfileName, importString);
            }
            catch (Exception e)
            {
                PluginLog.Error("加载配置文件错误：" + e.Message);
            }

            return false;
        }

        private bool ReloadCurrentProfile()
        {
            try
            {
                string importString = File.ReadAllText(CurrentProfilePath());
                if (ConfigurationManager.Instance.ImportProfile(_currentProfileName, _currentProfileName, importString, true))
                {
                    ConfigurationManager.Instance.SaveConfigurations(true);
                    return true;
                }
            }
            catch (Exception e)
            {
                PluginLog.Error("重载配置文件错误：" + e.Message);
            }

            return false;
        }

        public void UpdateCurrentProfile()
        {
            PlayerCharacter? player = Plugin.ClientState.LocalPlayer;
            if (player == null)
            {
                return;
            }

            uint jobId = player.ClassJob.Id;
            Profile currentProfile = CurrentProfile();
            JobRoles role = JobsHelper.RoleForJob(jobId);
            int index = JobsHelper.JobsByRole[role].IndexOf(jobId);

            if (index < 0)
            {
                return;
            }

            // current profile is enabled for this job, do nothing
            if (currentProfile.AutoSwitchEnabled && currentProfile.AutoSwitchData.IsEnabled(role, index))
            {
                return;
            }

            // find a profile that is enabled for this job
            foreach (Profile profile in Profiles.Values)
            {
                if (!profile.AutoSwitchEnabled || profile == currentProfile)
                {
                    continue;
                }

                // found a valid profile, switch to it
                if (profile.AutoSwitchData.IsEnabled(role, index))
                {
                    SwitchToProfile(profile.Name);
                    return;
                }
            }
        }

        public void CheckUpdateSwitchCurrentProfile(string specifiedProfile)
        {
            // found a valid profile, switch to it
            if (Profiles.ContainsKey(specifiedProfile))
            {
                SwitchToProfile(specifiedProfile);
            }
        }

        private string? SwitchToProfile(string profile, bool save = true)
        {
            // save if needed before switching
            if (save)
            {
                ConfigurationManager.Instance.SaveConfigurations();
            }

            string oldProfile = _currentProfileName;
            _currentProfileName = profile;
            Profile currentProfile = CurrentProfile();

            if (currentProfile.AttachHudEnabled && currentProfile.HudLayout != 0)
            {
                ChatHelper.SendChatMessage("/hudlayout " + currentProfile.HudLayout);
            }

            if (!LoadCurrentProfile(oldProfile))
            {
                _currentProfileName = oldProfile;
                return "无法加载配置文件\"" + profile + "\"！";
            }

            UpdateSelectedIndex();

            try
            {
                Save();
                Plugin.UiBuilder.RebuildFonts();
            }
            catch (Exception e)
            {
                PluginLog.Error("保存配置文件错误：" + e.Message);
                return "无法保存配置文件\"" + profile + "\"！";
            }

            return null;
        }

        private void UpdateSelectedIndex()
        {
            _selectedProfileIndex = Math.Max(0, Profiles.IndexOfKey(_currentProfileName));
        }

        private string CurrentProfilePath()
        {
            return Path.Combine(ProfilesPath, _currentProfileName + ".delvui");
        }

        private string? CloneProfile(string profileName, string newProfileName)
        {
            string srcPath = Path.Combine(ProfilesPath, profileName + ".delvui");
            string dstPath = Path.Combine(ProfilesPath, newProfileName + ".delvui");

            return CloneProfile(profileName, srcPath, newProfileName, dstPath);
        }

        private string? CloneProfile(string profileName, string srcPath, string newProfileName, string dstPath)
        {
            if (newProfileName.Length == 0)
            {
                return null;
            }

            if (Profiles.Keys.Contains(newProfileName))
            {
                return "配置文件\"" + newProfileName + "\"已存在！";
            }

            try
            {
                if (!File.Exists(srcPath))
                {
                    return "无法找到配置文件\"" + profileName + "\"！";
                }

                if (File.Exists(dstPath))
                {
                    return "配置文件\"" + newProfileName + "\"已存在！";
                }

                File.Copy(srcPath, dstPath);
                Profile newProfile = new Profile(newProfileName);
                Profiles.Add(newProfileName, newProfile);

                Save();
            }
            catch (Exception e)
            {
                PluginLog.Error("克隆配置文件错误：" + e.Message);
                return "尝试克隆配置文件时出错\"" + profileName + "\"！";
            }

            return null;
        }

        private string? RenameCurrentProfile(string newProfileName)
        {
            if (_currentProfileName == newProfileName || newProfileName.Length == 0)
            {
                return null;
            }

            if (Profiles.ContainsKey(newProfileName))
            {
                return "配置文件\"" + newProfileName + "\"已存在！";
            }

            string srcPath = Path.Combine(ProfilesPath, _currentProfileName + ".delvui");
            string dstPath = Path.Combine(ProfilesPath, newProfileName + ".delvui");

            try
            {

                if (File.Exists(dstPath))
                {
                    return "配置文件\"" + newProfileName + "\"已存在！";
                }

                File.Move(srcPath, dstPath);

                Profile profile = Profiles[_currentProfileName];
                profile.Name = newProfileName;

                Profiles.Remove(_currentProfileName);
                Profiles.Add(newProfileName, profile);

                _currentProfileName = newProfileName;

                Save();
            }
            catch (Exception e)
            {
                PluginLog.Error("重命名配置文件错误：" + e.Message);
                return "试图重命名配置文件时出错\"" + _currentProfileName + "\"！";
            }

            return null;
        }

        private string? Import(string newProfileName, string importString)
        {
            if (newProfileName.Length == 0)
            {
                return null;
            }

            if (Profiles.Keys.Contains(newProfileName))
            {
                return "配置文件\"" + newProfileName + "\"已存在！";
            }

            string dstPath = Path.Combine(ProfilesPath, newProfileName + ".delvui");

            try
            {
                if (File.Exists(dstPath))
                {
                    return "配置文件\"" + newProfileName + "\"已存在！";
                }

                File.WriteAllText(dstPath, importString);

                Profile newProfile = new Profile(newProfileName);
                Profiles.Add(newProfileName, newProfile);

                string? errorMessage = SwitchToProfile(newProfileName, false);

                if (errorMessage != null)
                {
                    Profiles.Remove(newProfileName);
                    File.Delete(dstPath);
                    Save();

                    return errorMessage;
                }
            }
            catch (Exception e)
            {
                PluginLog.Error("导入配置文件错误:" + e.Message);
                return "试图导入配置文件时出错\"" + newProfileName + "\"！";
            }

            return null;
        }

        private string? ImportFromClipboard(string newProfileName)
        {
            string importString = ImGui.GetClipboardText();
            if (importString.Length == 0)
            {
                return "无效的导入字符串!";
            }

            return Import(newProfileName, importString);
        }

        private void ImportFromFile(string newProfileName)
        {
            if (newProfileName.Length == 0)
            {
                return;
            }

            Action<bool, string> callback = (finished, path) =>
            {
                try
                {
                    if (finished && path.Length > 0)
                    {
                        string importString = File.ReadAllText(path);
                        _errorMessage = Import(newProfileName, importString);

                        if (_errorMessage == null)
                        {
                            _newProfileName = "";
                        }
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error("读取导入文件时出错！" + e.Message);
                    _errorMessage = "读取文件时出错!";
                }
            };

            _fileDialogManager.OpenFileDialog("选择要导入的DelvUI配置文件", "DelvUI Profile{.delvui}", callback);
        }

        private void ExportToFile(string newProfileName)
        {
            if (newProfileName.Length == 0)
            {
                return;
            }

            Action<bool, string> callback = (finished, path) =>
            {
                try
                {
                    string src = CurrentProfilePath();
                    if (finished && path.Length > 0 && src != path)
                    {
                        File.Copy(src, path, true);
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error("复制文件错误：" + e.Message);
                    _errorMessage = "导出文件错误!";
                }
            };

            _fileDialogManager.SaveFileDialog("保存配置文件", "DelvUI Profile{.delvui}", newProfileName + ".delvui", ".delvui", callback);
        }

        private string? DeleteProfile(string profileName)
        {
            if (!Profiles.ContainsKey(profileName))
            {
                return "找不到配置文件\"" + profileName + "\"！";
            }

            string path = Path.Combine(ProfilesPath, profileName + ".delvui");

            try
            {
                if (!File.Exists(path))
                {
                    return "找不到配置文件\"" + profileName + "\"！";
                }

                File.Delete(path);
                Profiles.Remove(profileName);

                Save();

                ConfigurationManager.Instance.OnProfileDeleted(profileName);

                if (_currentProfileName == profileName)
                {
                    return SwitchToProfile(DefaultProfileName, false);
                }
            }
            catch (Exception e)
            {
                PluginLog.Error("删除配置文件错误:" + e.Message);
                return "试图删除配置文件时出错\"" + profileName + "\"！";
            }

            return null;
        }

        private void Save()
        {
            string jsonString = JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.Objects
                }
            );

            Directory.CreateDirectory(ProfilesPath);
            File.WriteAllText(JsonPath, jsonString);
        }

        public bool Draw(ref bool changed)
        {
            string[] profiles = Profiles.Keys.ToArray();

            if (ImGui.BeginChild("配置文件", new Vector2(800, 600), false))
            {
                if (Profiles.Count == 0)
                {
                    ImGuiHelper.Tab();
                    ImGui.Text("配置文件未在路径中找到\"%appdata%/Roaming/XIVLauncher/pluginConfigs/DelvUI/Profiles/\"");
                    return false;
                }

                ImGui.PushItemWidth(408);
                ImGuiHelper.NewLineAndTab();
                if (ImGui.Combo("当前配置文件", ref _selectedProfileIndex, profiles, profiles.Length, 10))
                {
                    string newProfileName = profiles[_selectedProfileIndex];

                    if (_currentProfileName != newProfileName)
                    {
                        _errorMessage = SwitchToProfile(newProfileName);
                    }
                }

                // reset
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button("\uf2f9", new Vector2(0, 0)))
                {
                    _resetingProfileName = _currentProfileName;
                }
                ImGui.PopFont();
                ImGuiHelper.SetTooltip("重置");

                if (_currentProfileName != DefaultProfileName)
                {
                    // rename
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    if (ImGui.Button(FontAwesomeIcon.Pen.ToIconString()))
                    {
                        _renamingProfileName = _currentProfileName;
                    }
                    ImGui.PopFont();
                    ImGuiHelper.SetTooltip("重命名");

                    // delete
                    ImGui.SameLine();
                    ImGui.PushFont(UiBuilder.IconFont);
                    if (_currentProfileName != DefaultProfileName && ImGui.Button(FontAwesomeIcon.Trash.ToIconString()))
                    {
                        _deletingProfileName = _currentProfileName;
                    }
                    ImGui.PopFont();
                    ImGuiHelper.SetTooltip("删除");
                }

                // export to string
                ImGuiHelper.Tab();
                ImGui.SameLine();
                if (ImGui.Button("导出到剪贴板", new Vector2(200, 0)))
                {
                    string? exportString = ConfigurationManager.Instance.ExportCurrentConfigs();
                    if (exportString != null)
                    {
                        ImGui.SetClipboardText(exportString);
                        ImGui.OpenPopup("export_succes_popup");
                    }
                }

                // export success popup
                if (ImGui.BeginPopup("export_succes_popup"))
                {
                    ImGui.Text("Profile export string copied to clipboard!");
                    ImGui.EndPopup();
                }

                ImGui.SameLine();
                if (ImGui.Button("导出到文件", new Vector2(200, 0)))
                {
                    ExportToFile(_currentProfileName);
                }

                ImGuiHelper.NewLineAndTab();
                DrawAttachHudLayout(ref changed);

                ImGuiHelper.NewLineAndTab();
                DrawAutoSwitchSettings(ref changed);

                ImGuiHelper.DrawSeparator(1, 1);
                ImGuiHelper.Tab();
                ImGui.Text("创建一个新的配置文件：");

                ImGuiHelper.Tab();
                ImGui.PushItemWidth(408);
                ImGui.InputText("配置文件名称", ref _newProfileName, 200);

                ImGuiHelper.Tab();
                ImGui.PushItemWidth(200);
                ImGui.Combo("", ref _copyFromIndex, profiles, profiles.Length, 10);

                ImGui.SameLine();
                if (ImGui.Button("复制", new Vector2(200, 0)))
                {
                    _newProfileName = _newProfileName.Trim();
                    if (_newProfileName.Length == 0)
                    {
                        ImGui.OpenPopup("import_error_popup");
                    }
                    else
                    {
                        _errorMessage = CloneProfile(profiles[_copyFromIndex], _newProfileName);

                        if (_errorMessage == null)
                        {
                            _errorMessage = SwitchToProfile(_newProfileName);
                            _newProfileName = "";
                        }
                    }
                }

                ImGuiHelper.NewLineAndTab();
                if (ImGui.Button("从剪贴板导入", new Vector2(200, 0)))
                {
                    _newProfileName = _newProfileName.Trim();
                    if (_newProfileName.Length == 0)
                    {
                        ImGui.OpenPopup("import_error_popup");
                    }
                    else
                    {
                        _errorMessage = ImportFromClipboard(_newProfileName);

                        if (_errorMessage == null)
                        {
                            _newProfileName = "";
                        }
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button("从文件导入", new Vector2(200, 0)))
                {
                    _newProfileName = _newProfileName.Trim();
                    if (_newProfileName.Length == 0)
                    {
                        ImGui.OpenPopup("import_error_popup");
                    }
                    else
                    {
                        ImportFromFile(_newProfileName);
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button("浏览配置文件", new Vector2(200, 0)))
                {
                    Utils.OpenUrl("https://wago.io/delvui");
                }

                // no name popup
                if (ImGui.BeginPopup("import_error_popup"))
                {
                    ImGui.Text("请输入新配置文件的名称!");
                    ImGui.EndPopup();
                }
            }

            ImGui.EndChild();

            // error message
            if (_errorMessage != null)
            {
                if (ImGuiHelper.DrawErrorModal(_errorMessage))
                {
                    _errorMessage = null;
                }
            }

            // delete confirmation
            if (_deletingProfileName != null)
            {
                string[] lines = new string[] { "你确定要删除配置文件吗？", "\u2002- " + _deletingProfileName };
                var (didConfirm, didClose) = ImGuiHelper.DrawConfirmationModal("删除？", lines);

                if (didConfirm)
                {
                    _errorMessage = DeleteProfile(_deletingProfileName);
                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _deletingProfileName = null;
                }
            }

            // reset confirmation
            if (_resetingProfileName != null)
            {
                string[] lines = new string[] { "你确定要重置配置文件吗？", "\u2002- " + _resetingProfileName };
                var (didConfirm, didClose) = ImGuiHelper.DrawConfirmationModal("重置？", lines);

                if (didConfirm)
                {
                    ResetProfileToDefault(_resetingProfileName, true);
                    ReloadCurrentProfile();

                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _resetingProfileName = null;
                }
            }

            // rename modal
            if (_renamingProfileName != null)
            {
                var (didConfirm, didClose) = ImGuiHelper.DrawInputModal("重命名", "为配置文件输入一个新名称：", ref _renamingProfileName);

                if (didConfirm)
                {
                    _errorMessage = RenameCurrentProfile(_renamingProfileName);

                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _renamingProfileName = null;
                }
            }

            _fileDialogManager.Draw();

            return false;
        }

        private void DrawAutoSwitchSettings(ref bool changed)
        {
            Profile profile = CurrentProfile();

            changed |= ImGui.Checkbox("为特定职业自动切换", ref profile.AutoSwitchEnabled);

            if (!profile.AutoSwitchEnabled)
            {
                return;
            }

            AutoSwitchData data = profile.AutoSwitchData;
            Vector2 cursorPos = ImGui.GetCursorPos() + new Vector2(14, 14);
            Vector2 originalPos = cursorPos;
            float maxY = 0;

            JobRoles[] roles = (JobRoles[])Enum.GetValues(typeof(JobRoles));

            foreach (JobRoles role in roles)
            {
                if (role == JobRoles.Unknown) { continue; }
                if (!data.Map.ContainsKey(role)) { continue; }

                bool roleValue = data.GetRoleEnabled(role);
                string roleName = JobsHelper.RoleNames[role];

                ImGui.SetCursorPos(cursorPos);
                if (ImGui.Checkbox(roleName, ref roleValue))
                {
                    data.SetRoleEnabled(role, roleValue);
                    changed = true;
                }

                cursorPos.Y += 40;
                int jobCount = data.Map[role].Count;

                for (int i = 0; i < jobCount; i++)
                {
                    maxY = Math.Max(cursorPos.Y, maxY);
                    uint jobId = JobsHelper.JobsByRole[role][i];
                    bool jobValue = data.Map[role][i];
                    string jobName = JobsHelper.JobNames[jobId];

                    ImGui.SetCursorPos(cursorPos);
                    if (ImGui.Checkbox(jobName, ref jobValue))
                    {
                        data.Map[role][i] = jobValue;
                        changed = true;
                    }

                    cursorPos.Y += 30;
                }

                cursorPos.X += 100;
                cursorPos.Y = originalPos.Y;
            }

            ImGui.SetCursorPos(new Vector2(originalPos.X, maxY + 30));
        }

        private void DrawAttachHudLayout(ref bool changed)
        {
            Profile profile = CurrentProfile();

            changed |= ImGui.Checkbox("将HUD布局添加到此配置文件", ref profile.AttachHudEnabled);

            if (!profile.AttachHudEnabled)
            {
                profile.HudLayout = 0;
                return;
            }

            int hudLayout = profile.HudLayout;

            ImGui.Text("\u2002\u2002\u2514");

            for (int i = 1; i <= 4; i++)
            {
                ImGui.SameLine();
                bool hudLayoutEnabled = hudLayout == i;
                if (ImGui.Checkbox("Hud布局" + i, ref hudLayoutEnabled))
                {
                    profile.HudLayout = i;
                    changed = true;
                }
            }

        }
    }

    // fake config object
    [Disableable(false)]
    [Exportable(false)]
    [Shareable(false)]
    [Resettable(false)]
    public class ProfilesConfig : PluginConfigObject
    {
        public new static ProfilesConfig DefaultConfig() { return new ProfilesConfig(); }
    }

    // fake config page node
    public class ProfilesConfigPageNode : ConfigPageNode
    {
        public ProfilesConfigPageNode()
        {
            ConfigObject = new ProfilesConfig();
        }

        public override bool Draw(ref bool changed)
        {
            return ProfilesManager.Instance?.Draw(ref changed) ?? false;
        }
    }
}
