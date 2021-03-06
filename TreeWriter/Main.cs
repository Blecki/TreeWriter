﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TreeWriterWF
{
    public partial class Main : Form
    {
        private Model ProjectModel = new Model();
        private SettingsEditor SettingsEditor = null;
        private Help HelpWindow = null;

        public Main()
        {
            InitializeComponent();

            this.dockPanel.Theme = new VS2012LightTheme();
            ProjectModel.LoadSettings(this);

            Font = Settings.GlobalSettings.SystemFont;

            var dockPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BleckiTreeWriter\\dock.txt";

            try
            {
                if (System.IO.File.Exists(dockPath))
                    dockPanel.LoadFromXml(dockPath, DeserializeDockContent);
            }
            catch (Exception) { }
        }

        IDockContent DeserializeDockContent(string persistString)
        {
            var openCommand = new Commands.OpenPath(persistString, Commands.OpenCommand.OpenStyles.InitialLoad);
            openCommand.Execute(ProjectModel, this);
            if (openCommand.Panel != null)
                openCommand.Panel.InvokeCommand += ProcessControllerCommand;
            return openCommand.Panel;
        }

        public void UpdateRecentDocuments()
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            recentToolStripMenuItem.DropDownItems.Add(recentClearList);
            if (Settings.GlobalSettings.RecentDocuments.Count > 0)
                recentToolStripMenuItem.DropDownItems.Add(recentToolStripSeparator1);

            foreach (var recentDocument in Settings.GlobalSettings.RecentDocuments)
            {
                var item = new ToolStripMenuItem(recentDocument.Replace("&","&&"));
                item.Click += (sender, args) =>
                    {
                        ProcessControllerCommand(new Commands.OpenPath(item.Text.Replace("&&","&"), Commands.OpenCommand.OpenStyles.CreateView));
                    };
                recentToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        public void OpenControllerPanel(DockablePanel Panel, DockState Where)
        {
            Panel.InvokeCommand += ProcessControllerCommand;
            Panel.Show(dockPanel, Where);
        }

        internal void ProcessControllerCommand(ICommand Command)
        {
            // TODO: Queue these up or something?
            // TODO: Implement undo
            Command.Execute(ProjectModel, this);
        }

        private void OpenHelpWindow(String To)
        {
            if (HelpWindow == null || HelpWindow.Open == false)
            {
                HelpWindow = new Help();
                OpenControllerPanel(HelpWindow, DockState.Document);
            }
            HelpWindow.Activate();
            HelpWindow.Navigate(To);
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ProcessControllerCommand(new Commands.OpenCommand<FolderDocument>(folderDialog.SelectedPath,
                    Commands.OpenCommand.OpenStyles.CreateView));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            var closeCommand = new Commands.CloseApplication();
            ProcessControllerCommand(closeCommand);
            if (closeCommand.Cancel == false)
            {
                ProjectModel.SaveSettings();
                dockPanel.SaveAsXml(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BleckiTreeWriter\\dock.txt");
            }
            else
                e.Cancel = true;
        }

        private void saveDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessControllerCommand(new Commands.SaveOpenDocuments());
        }

        private void closeAllDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessControllerCommand(new Commands.CloseAllEditors());
        }

        private void openDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ProcessControllerCommand(new Commands.OpenPath(fileDialog.FileName,
                    Commands.OpenCommand.OpenStyles.CreateView));
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SettingsEditor == null || SettingsEditor.Open == false)
            {
                SettingsEditor = new SettingsEditor(ProjectModel);
                OpenControllerPanel(SettingsEditor, DockState.Document);
            }
            SettingsEditor.Activate();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenHelpWindow("about");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHelpWindow("main");
        }

        private void newManuscriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Manuscripts (*.ms)|*.ms";
            fileDialog.CheckFileExists = false;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProcessControllerCommand(new Commands.CreateNewDocumentAtPath(fileDialog.FileName));
                ProcessControllerCommand(new Commands.OpenPath(fileDialog.FileName, Commands.OpenCommand.OpenStyles.CreateView));
            }
        }

        private void newTextDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text files (*.txt)|*.txt";
            fileDialog.CheckFileExists = false;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProcessControllerCommand(new Commands.CreateNewDocumentAtPath(fileDialog.FileName));
                ProcessControllerCommand(new Commands.OpenPath(fileDialog.FileName, Commands.OpenCommand.OpenStyles.CreateView));
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings.GlobalSettings.RecentDocuments.Clear();
            UpdateRecentDocuments();
        }
    }
}
