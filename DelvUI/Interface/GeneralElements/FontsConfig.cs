﻿using Dalamud.Interface;
using Dalamud.Interface.ImGuiFileDialog;
using DelvUI.Config;
using DelvUI.Config.Attributes;
using DelvUI.Helpers;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.GameFonts;
using Dalamud.Logging;
using DelvUI.Enums;
using DelvUI.Interface.Bars;

namespace DelvUI.Interface.GeneralElements
{
    public struct FontData
    {
        public string Name;
        public int Size;

        public FontData(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }

    [Disableable(false)]
    [Section("定制")]
    [SubSection("字体", 0)]
    public class FontsConfig : PluginConfigObject
    {
        public new static FontsConfig DefaultConfig() { return new FontsConfig(); }

        public string FontsPath = "C:\\";
        [JsonIgnore] public string ValidatedFontsPath => ValidatePath(FontsPath);

        public SortedList<string, FontData> Fonts = new SortedList<string, FontData>();
        public bool SupportChineseCharacters = false;
        public bool SupportKoreanCharacters = false;
        [JsonIgnore] public readonly Dictionary<string, string> GameFontMap = new Dictionary<string, string>()
        {
            {"Axis", "axis-ffxiv"},
            {"Jupiter", "jupiter-ffxiv"},
            {"JupiterNumeric", "jupiter-numeric-ffxiv"},
            {"MiedingerMid", "meidinger-ffxiv"},
            {"Meidinger", "meidinger-numberic-ffxiv"},
            {"TrumpGothic", "trumpgothic-ffxiv"},
            
        };

        [JsonIgnore] public static readonly List<string> DefaultFontsKeys = new List<string>() { "Expressway_24", "Expressway_20", "Expressway_16" };

        [JsonIgnore] public static string DefaultBigFontKey => DefaultFontsKeys[0];
        [JsonIgnore] public static string DefaultMediumFontKey => DefaultFontsKeys[1];
        [JsonIgnore] public static string DefaultSmallFontKey => DefaultFontsKeys[2];

        [JsonIgnore] private int _inputFont = 0;
        [JsonIgnore] private int _inputSize = 23;

        [JsonIgnore] private string[] _fonts = null!;
        [JsonIgnore] private string[] _sizes = null!;

        [JsonIgnore] private int _removingIndex = -1;
        [JsonIgnore] private int _applyingIndex = -1!;

        [JsonIgnore] private FileDialogManager _fileDialogManager = new FileDialogManager();

        public FontsConfig()
        {
            ReloadFonts();

            // default fonts
            foreach (string key in DefaultFontsKeys)
            {
                if (!Fonts.ContainsKey(key))
                {
                    string[] str = key.Split("_", StringSplitOptions.RemoveEmptyEntries);
                    var defaultFont = new FontData(str[0], int.Parse(str[1]));
                    Fonts.Add(key, defaultFont);
                }
            }

            // sizes
            _sizes = new string[100];
            for (int i = 0; i < _sizes.Length; i++)
            {
                _sizes[i] = (i + 1).ToString();
            }
        }

        private bool IsDefaultFont(string key)
        {
            return DefaultFontsKeys.Contains(key);
        }

        private string ValidatePath(string path)
        {
            if (path.EndsWith("\\") || path.EndsWith("/"))
            {
                return path;
            }

            return path + "\\";
        }

        private string[] FontsFromPath(string path)
        {
            string[] fonts;
            try
            {
                fonts = Directory.GetFiles(path, "*.ttf");
            }
            catch
            {
                fonts = new string[0];
            }

            for (int i = 0; i < fonts.Length; i++)
            {
                fonts[i] = fonts[i]
                    .Replace(path, "")
                    .Replace(".ttf", "")
                    .Replace(".TTF", "");
            }

            return fonts;
        }
        
        private string[] FontsFromGame()
        {
            string[] gameFontArray = Enum.GetNames(typeof(GameFontFamily)).Skip(1).ToArray();
            string[] fonts = new string[gameFontArray.Length];

            for (int i = 0; i < gameFontArray.Length; i++)
            {
                fonts[i] = GameFontMap[gameFontArray[i]];
            }

            return fonts;
        }

        private void ReloadFonts()
        {
            string defaultFontsPath = ValidatePath(FontsManager.Instance.DefaultFontsPath);
            string[] defaultFonts = FontsFromPath(defaultFontsPath);
            string[] gameFonts = FontsFromGame();
            string[] userFonts = FontsFromPath(ValidatedFontsPath);

            _fonts = new string[defaultFonts.Length + gameFonts.Length + userFonts.Length];
            defaultFonts.CopyTo(_fonts, 0);
            gameFonts.CopyTo(_fonts, defaultFonts.Length);
            userFonts.CopyTo(_fonts, defaultFonts.Length + gameFonts.Length);
        }

        private bool AddNewEntry(int font, int size)
        {
            if (font < 0 || font > _fonts.Length)
            {
                return false;
            }

            if (size <= 0 || size > _sizes.Length)
            {
                return false;
            }

            string fontName = _fonts[font];
            string key = fontName + "_" + size.ToString();

            if (Fonts.ContainsKey(key))
            {
                return false;
            }

            FontData fontData = new FontData(fontName, size);
            Fonts.Add(key, fontData);

            Plugin.UiBuilder.RebuildFonts();

            return true;
        }

        private void SelectFolder()
        {
            Action<bool, string> callback = (finished, path) =>
            {
                if (finished && path.Length > 0)
                {
                    FontsPath = path;
                    ReloadFonts();
                }
            };

            _fileDialogManager.OpenFolderDialog("选择字体文件夹", callback);
        }

        [ManualDraw]
        public bool Draw(ref bool changed)
        {
            ImGuiTableFlags flags =
                ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Borders |
                ImGuiTableFlags.BordersOuter |
                ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.SizingFixedSame;

            if (ImGui.BeginChild("字体", new Vector2(400, 500), false, ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                if (_fonts.Length == 0)
                {
                    ImGuiHelper.Tab();
                    ImGui.Text("此处未找到默认字体 \"%appdata%/Roaming/XIVLauncher/InstalledPlugins/DelvUI/Media/Fonts/Expressway.ttf\"");
                    return false;
                }

                ImGuiHelper.NewLineAndTab();
                if (ImGui.InputText("路径", ref FontsPath, 200, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    changed = true;
                    ReloadFonts();
                }

                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(FontAwesomeIcon.Folder.ToIconString(), new Vector2(0, 0)))
                {
                    SelectFolder();
                }
                ImGui.PopFont();

                ImGuiHelper.Tab();
                ImGui.Combo("字体##font", ref _inputFont, _fonts, _fonts.Length, 10);

                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button("\uf2f9", new Vector2(0, 0)))
                {
                    ReloadFonts();
                }
                ImGui.PopFont();

                ImGuiHelper.Tab();
                ImGui.Combo("尺寸##size", ref _inputSize, _sizes, _sizes.Length, 10);

                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(FontAwesomeIcon.Plus.ToIconString(), new Vector2(0, 0)))
                {
                    changed |= AddNewEntry(_inputFont, _inputSize + 1);
                }
                ImGui.PopFont();

                ImGuiHelper.NewLineAndTab();
                if (ImGui.BeginTable("表格", 3, flags, new Vector2(326, 300)))
                {
                    ImGui.TableSetupColumn("名字", ImGuiTableColumnFlags.WidthStretch, 0, 0);
                    ImGui.TableSetupColumn("尺寸", ImGuiTableColumnFlags.WidthFixed, 0, 1);
                    ImGui.TableSetupColumn("动作", ImGuiTableColumnFlags.WidthFixed, 0, 2);

                    ImGui.TableSetupScrollFreeze(0, 1);
                    ImGui.TableHeadersRow();

                    for (int i = 0; i < Fonts.Count; i++)
                    {
                        var key = Fonts.Keys[i];
                        var fontData = Fonts.Values[i];

                        ImGui.PushID(i.ToString());
                        ImGui.TableNextRow(ImGuiTableRowFlags.None);

                        // icon
                        if (ImGui.TableSetColumnIndex(0))
                        {
                            ImGui.Text(fontData.Name);
                        }

                        // id
                        if (ImGui.TableSetColumnIndex(1))
                        {
                            ImGui.Text(fontData.Size.ToString());
                        }

                        // remove
                        if (ImGui.TableSetColumnIndex(2))
                        {
                            ImGui.PushFont(UiBuilder.IconFont);
                            ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
                            ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);
                            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);

                            if (ImGui.Button(FontAwesomeIcon.ArrowAltCircleUp.ToIconString()))
                            {
                                _applyingIndex = i;
                            }

                            if (!IsDefaultFont(key))
                            {
                                ImGui.SameLine();
                                if (ImGui.Button(FontAwesomeIcon.Trash.ToIconString()))
                                {
                                    _removingIndex = i;
                                }
                            }

                            ImGui.PopFont();
                            ImGui.PopStyleColor(3);
                        }
                        ImGui.PopID();
                    }

                    ImGui.EndTable();
                }

                ImGuiHelper.NewLineAndTab();
                if (ImGui.Checkbox("支持中文", ref SupportChineseCharacters))
                {
                    changed = true;
                    Plugin.UiBuilder.RebuildFonts();
                }

                ImGui.SameLine();
                if (ImGui.Checkbox("支持韩文", ref SupportKoreanCharacters))
                {
                    changed = true;
                    Plugin.UiBuilder.RebuildFonts();
                }
            }

            // apply confirmation
            if (_applyingIndex >= 0)
            {
                string[] lines = new string[] { "你确定你想要将这种字体应用到", "使用相同尺寸字体的所有标签？" };
                var (didConfirm, didClose) = ImGuiHelper.DrawConfirmationModal("应用到所有标签？", lines);

                if (didConfirm)
                {
                    var (key, font) = Fonts.ElementAt(_applyingIndex);

                    List<LabelConfig> labelConfigs = ConfigurationManager.Instance.GetObjects<LabelConfig>();
                    foreach (LabelConfig label in labelConfigs)
                    {
                        if (label.FontID != null && Fonts.TryGetValue(label.FontID, out FontData value))
                        {
                            if (font.Size == value.Size)
                            {
                                label.FontID = key;
                            }
                        }
                    }

                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _applyingIndex = -1;
                }
            }

            // delete confirmation
            if (_removingIndex >= 0)
            {
                string[] lines = new string[] { "你确定要删除此字体吗？" };
                var (didConfirm, didClose) = ImGuiHelper.DrawConfirmationModal("删除自定义字体？", lines);

                if (didConfirm)
                {
                    Fonts.RemoveAt(_removingIndex);
                    Plugin.UiBuilder.RebuildFonts();
                    changed = true;
                }

                if (didConfirm || didClose)
                {
                    _removingIndex = -1;
                }
            }

            ImGui.EndChild();

            _fileDialogManager.Draw();

            return false;
        }
    }
}
