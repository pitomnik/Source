using System.Windows.Input;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZoneIDTrimmer.Shared;
using ZoneIDTrimmer.Shell.Properties;

namespace ZoneIDTrimmer.Shell
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    [COMServerAssociation(AssociationType.Directory)]
    [COMServerAssociation(AssociationType.Drive)]
    public class ZoneIDContextMenu : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var isShiftKeyDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            var menuStrip = new ContextMenuStrip();

            try
            {
                var menuItem = CreateMenuItem(isShiftKeyDown);

                menuStrip.Items.Add(menuItem);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("Failed to create menu.", ex);
            }

            return menuStrip;
        }

        private ToolStripMenuItem CreateMenuItem(bool asAdmin = false)
        {
            var iconSize = GetMenuIconSize();

            LogMaster.Instance.DebugFormat("Menu icon size: {0}.", iconSize);

            var menuItem = new ToolStripMenuItem
            {
                Text = asAdmin ? Resources.MenuTextAsAdmin : Resources.MenuText,
                ToolTipText = asAdmin ? Resources.MenuTooltipAsAdmin : Resources.MenuTooltip,
                Image = new Icon(Shared.Properties.Resources.Trimmer, iconSize).ToBitmap()
            };

            menuItem.Click += (sender, args) => TryTrimSelectedPaths(asAdmin);

            return menuItem;
        }

        private Size GetMenuIconSize()
        {
            var customIconSize = ConfigManager.Instance.ReadValue(ConfigKeys.MenuIconSize, 0);
            
            return customIconSize > 0 ? new Size(customIconSize, customIconSize) : SystemInformation.MenuCheckSize;
        }

        private void TryTrimSelectedPaths(bool asAdmin = false)
        {
            try
            {
                TrimSelectedPaths(asAdmin);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("TrimSelectedPaths failed.", ex);
            }
        }

        private void TrimSelectedPaths(bool asAdmin = false)
        {
            if (!SelectedItemPaths.Any()) return;

            if (LogMaster.Instance.IsDebugEnabled)
            {
                LogMaster.Instance.Debug("Selected paths:");

                foreach (var path in SelectedItemPaths)
                {
                    LogMaster.Instance.DebugFormat("\t{0}", path);
                }
            }

            var arguments = String.Join(" ", SelectedItemPaths.Select(x => PathTools.Normalize(x, true)));

            LogMaster.Instance.DebugFormat("Process arguments: {0}", arguments);

            try
            {
                var processStartInfo = new ProcessStartInfo("ZoneIDTrimmer.Tool.exe", arguments);

                if (asAdmin && Environment.OSVersion.Version.Major >= 6)
                {
                    processStartInfo.Verb = "runas";
                }

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("Failed to start process.", ex);
            }
        }
    }
}
