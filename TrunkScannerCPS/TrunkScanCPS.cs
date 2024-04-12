using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace TrunkScannerCPS
{
    public partial class Form1 : Form
    {
        private Codeplug currentCodeplug;
        private static string appVersion = "R01.00.00";
        private AppType currentAppType = AppType.CPS;

        public Form1()
        {
            InitializeComponent();
            this.Text = $"TrunkScan {currentAppType.Name}. Version: {appVersion}";
            AdjustFieldEditability();
        }

        private void AdjustFieldEditability()
        {
            bool isSerialNumberEnabled = false;
            bool isModelEnabled = false;
            bool isCodeplugVersionEnabled = false;
            bool isLastProgramSrcEnabled = false;

            switch (currentAppType.Name)
            {
                case "CPS":
                    // Nothing editable in CPS
                    break;
                case "Depot":
                    // For Depot, serial and model can change
                    MessageBox.Show("Depot Mode enabled!");
                    isSerialNumberEnabled = true;
                    isModelEnabled = true;
                    break;
                case "Labtool":
                case "PhpSplutions":
                    // For Labtool and PhpSplutions, all fields are enabled
                    MessageBox.Show("Lab mode enabled!");
                    isSerialNumberEnabled = true;
                    isModelEnabled = true;
                    isCodeplugVersionEnabled = true;
                    isLastProgramSrcEnabled = true;
                    break;
            }

            serialNumberBox.Enabled = isSerialNumberEnabled;
            modelBox.Enabled = isModelEnabled;
            codeplugVersionBox.Enabled = isCodeplugVersionEnabled;
            lastProgramSrcBox.Enabled = isLastProgramSrcEnabled;
        }

        private void LoadCodeplug(string json)
        {
            currentCodeplug = JsonConvert.DeserializeObject<Codeplug>(json);

            if (!currentCodeplug.IsValid())
            {
                MessageBox.Show("invalid Codeplug Detected");
                return;
            }

            if (currentCodeplug.IsKilled())
            {
                MessageBox.Show("Radio has been KILLED! Contact system admin!");
                return;
            }

            if (currentCodeplug.IsTrunkingInhibited())
            {
                MessageBox.Show("Radio has been INHIBITED! Contact PhpSplutions!");
                return;
            }

            treeView1.Nodes.Clear();

            TreeNode zonesParentNode = new TreeNode("Zones");
            treeView1.Nodes.Add(zonesParentNode);

            foreach (var zone in currentCodeplug.Zones)
            {
                TreeNode zoneNode = new TreeNode(zone.Name);
                foreach (var channel in zone.Channels)
                {
                    zoneNode.Nodes.Add(new TreeNode($"{channel.Alias} ({channel.Tgid})"));
                }
                zonesParentNode.Nodes.Add(zoneNode);
            }

            zonesParentNode.Expand();

            TreeNode scanListsParentNode = new TreeNode("Scan Lists");
            treeView1.Nodes.Add(scanListsParentNode);

            foreach (var scanList in currentCodeplug.ScanLists)
            {
                TreeNode scanListNode = new TreeNode(scanList.Name) { Tag = scanList };
                foreach (var item in scanList.Items)
                {
                    scanListNode.Nodes.Add(new TreeNode($"{item.Alias} ({item.Tgid})"));
                }
                scanListsParentNode.Nodes.Add(scanListNode);
            }


            scanListsParentNode.Expand();

            PopulateTtsEnableComboBox(currentCodeplug.TtsEnabled);

            serialNumberBox.Text = currentCodeplug.SerialNumber;
            modelBox.Text = currentCodeplug.ModelNumber;
            codeplugVersionBox.Text = currentCodeplug.CodeplugVersion;

            CodeplugSource source = (CodeplugSource)currentCodeplug.LastProgramSource;

            lastProgramSrcBox.Text = source.ToString();
        }

        private void btnLoadCodeplug_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Codeplug File",
                Filter = "JSON Files|*.json|All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileContent = File.ReadAllText(openFileDialog.FileName);

                    LoadCodeplug(fileContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            txtZoneName.Clear();
            txtChannelName.Clear();
            txtTgid.Clear();
            txtScanListName.Clear();

            if (e.Node.Level == 1 && e.Node.Parent != null && e.Node.Parent.Text == "Zones")
            {
                txtZoneName.Text = e.Node.Text;
                Zone selectedZone = currentCodeplug.Zones.FirstOrDefault(z => z.Name == e.Node.Text);

                if (selectedZone != null)
                {
                    PopulateScanListComboBox(selectedZone.ScanListName);
                }
            }
            else if (e.Node.Level == 2)
            {
                var parts = e.Node.Text.Split(new string[] { " (" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    txtChannelName.Text = parts[0];
                    txtTgid.Text = parts[1].TrimEnd(')');
                }
            }
            else if (e.Node.Level == 1 && e.Node.Parent != null && e.Node.Parent.Text == "Scan Lists")
            {
                txtScanListName.Text = e.Node.Text;
            }
        }

        private void PopulateScanListComboBox(string selectedScanListName)
        {
            cmbScanList.Items.Clear();

            cmbScanList.Items.Add("<None>");
            foreach (var scanList in currentCodeplug.ScanLists)
            {
                cmbScanList.Items.Add(scanList.Name);
            }

            if (string.IsNullOrEmpty(selectedScanListName))
            {
                cmbScanList.SelectedItem = "<None>";
            }
            else
            {
                cmbScanList.SelectedItem = selectedScanListName;
            }
        }

        private void PopulateTtsEnableComboBox(bool enabled)
        {
            cmbTtsEnabled.Items.Clear();

            cmbTtsEnabled.Items.Add("True");
            cmbTtsEnabled.Items.Add("False");

            cmbTtsEnabled.SelectedItem = enabled ? "True" : "False";
        }


        private void btnDeleteChannel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 2)
            {
                var zoneNode = treeView1.SelectedNode.Parent;
                var channelNode = treeView1.SelectedNode;

                var zone = currentCodeplug.Zones.FirstOrDefault(z => z.Name == zoneNode.Text);
                if (zone != null)
                {
                    var channelAlias = channelNode.Text.Split(new string[] { " (" }, StringSplitOptions.None)[0];

                    var channel = zone.Channels.FirstOrDefault(c => c.Alias == channelAlias);
                    if (channel != null)
                    {
                        zone.Channels.Remove(channel);

                        zoneNode.Nodes.Remove(channelNode);
                    }
                    else
                    {
                        MessageBox.Show("Channel not found in the codeplug data model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Zone not found in the codeplug data model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddChannel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
            {
                var zoneName = treeView1.SelectedNode.Text;
                var zone = currentCodeplug.Zones.FirstOrDefault(z => z.Name == zoneName);
                if (zone != null)
                {
                    var newChannel = new Channel
                    {
                        Alias = txtChannelName.Text,
                        Tgid = txtTgid.Text
                    };

                    zone.Channels.Add(newChannel);

                    var channelNode = new TreeNode($"{newChannel.Alias} ({newChannel.Tgid})");
                    treeView1.SelectedNode.Nodes.Add(channelNode);
                    treeView1.SelectedNode.Expand();
                }
                else
                {
                    MessageBox.Show("Selected zone not found in the codeplug data model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnSaveChannel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 2)
            {
                var selectedZoneNode = treeView1.SelectedNode.Parent;
                var selectedChannelNode = treeView1.SelectedNode;

                var selectedZone = currentCodeplug.Zones.FirstOrDefault(z => z.Name == selectedZoneNode.Text);
                if (selectedZone != null)
                {
                    var channelToSave = selectedZone.Channels.FirstOrDefault(c => $"{c.Alias} ({c.Tgid})" == selectedChannelNode.Text);
                    if (channelToSave != null)
                    {
                        channelToSave.Alias = txtChannelName.Text;
                        channelToSave.Tgid = txtTgid.Text;

                        selectedChannelNode.Text = $"{channelToSave.Alias} ({channelToSave.Tgid})";
                    }
                    else
                    {
                        MessageBox.Show("Selected channel not found in the codeplug data model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Parent zone of the selected channel not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a channel to save its details.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAddZone_Click(object sender, EventArgs e)
        {
            var zonesNode = treeView1.Nodes[0];

            string zoneName = "New zone";

            if (!string.IsNullOrEmpty(zoneName))
            {
                if (currentCodeplug.Zones.Any(z => z.Name.Equals(zoneName, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("A zone with this name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var newZone = new Zone { Name = zoneName };
                currentCodeplug.Zones.Add(newZone);

                var newZoneNode = new TreeNode(zoneName);
                zonesNode.Nodes.Add(newZoneNode);

                zonesNode.Expand();
            }
        }

        private void btnDeleteZone_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
            {
                var zoneToDelete = currentCodeplug.Zones.FirstOrDefault(z => z.Name == treeView1.SelectedNode.Text);
                if (zoneToDelete != null)
                {
                    currentCodeplug.Zones.Remove(zoneToDelete);
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
        }

        private void btnSaveZoneName_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1 && treeView1.SelectedNode.Parent.Text == "Zones")
            {
                string updatedZoneName = txtZoneName.Text.Trim();
                string selectedScanListName = cmbScanList.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(updatedZoneName))
                {
                    MessageBox.Show("Zone name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentCodeplug.Zones.Any(z => z.Name.Equals(updatedZoneName, StringComparison.OrdinalIgnoreCase) && z.Name != treeView1.SelectedNode.Text))
                {
                    MessageBox.Show("Another zone with this name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Zone zoneToUpdate = currentCodeplug.Zones.FirstOrDefault(z => z.Name == treeView1.SelectedNode.Text);
                if (zoneToUpdate != null)
                {
                    zoneToUpdate.Name = updatedZoneName;
                    zoneToUpdate.ScanListName = selectedScanListName;

                    treeView1.SelectedNode.Text = updatedZoneName;

                    MessageBox.Show("Zone updated successfully.", "Success");
                }
                else
                {
                    MessageBox.Show("Selected zone not found in the codeplug data model.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void btnSaveCodeplug_Click(object sender, EventArgs e)
        {
            switch (currentAppType.Name)
            {
                case "CPS":
                    currentCodeplug.LastProgramSource = (int)CodeplugSource.CPS;
                    break;
                case "Depot":
                    currentCodeplug.LastProgramSource = (int)CodeplugSource.Depot;
                    break;
                case "Labtool":
                    currentCodeplug.LastProgramSource = (int)CodeplugSource.Labtool;
                    break;
                case "PhpSplutions":
                    currentCodeplug.LastProgramSource = (int)CodeplugSource.PhpSplutions;
                    break;
                default:
                    currentCodeplug.LastProgramSource = (int)CodeplugSource.CPS;
                    break;
            }

            currentCodeplug.CodeplugVersion = appVersion;
            currentCodeplug.ModelNumber = modelBox.Text;
            currentCodeplug.CodeplugVersion = codeplugVersionBox.Text;

            if (cmbTtsEnabled.SelectedItem.ToString() == "True")
            {
                currentCodeplug.TtsEnabled = true;
            } 
            else if (cmbTtsEnabled.SelectedItem.ToString() == "False")
            {
                currentCodeplug.TtsEnabled = false;
            }
            else
            {
                currentCodeplug.TtsEnabled = true;
            }

            CodeplugSource source = (CodeplugSource)currentCodeplug.LastProgramSource;

            codeplugVersionBox.Text = appVersion;
            lastProgramSrcBox.Text = source.ToString();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Codeplug File",
                Filter = "JSON Files|*.json|All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var json = JsonConvert.SerializeObject(currentCodeplug, Formatting.Indented);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        private void btnRenameScanList_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null &&
                treeView1.SelectedNode.Parent.Text == "Scan Lists" && treeView1.SelectedNode.Tag is ScanList scanList)
            {
                string newName = txtScanListName.Text.Trim();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    scanList.Name = newName;
                    treeView1.SelectedNode.Text = newName;
                }
                else
                {
                    MessageBox.Show("Scan list name cannot be empty.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select a scan list to rename.", "Warning");
            }
        }




        private void btnAddChannelToScanList_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag is ScanList scanList)
            {
                string channelAlias = txtChannelName.Text;
                string channelTgid = txtTgid.Text;

                if (!string.IsNullOrWhiteSpace(channelAlias) && !string.IsNullOrWhiteSpace(channelTgid))
                {
                    ScanListItem newItem = new ScanListItem { Alias = channelAlias, Tgid = channelTgid };
                    scanList.Items.Add(newItem);

                    TreeNode newNode = new TreeNode($"{newItem.Alias} ({newItem.Tgid})");
                    treeView1.SelectedNode.Nodes.Add(newNode);
                }
                else
                {
                    MessageBox.Show("Channel details cannot be empty.", "Error");
                }
            }
        }

        private void btnRemoveChannelFromScanList_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Parent?.Tag is ScanList scanList)
            {
                var selectedItem = scanList.Items.FirstOrDefault(i => i.Alias == txtChannelName.Text && i.Tgid == txtTgid.Text);
                if (selectedItem != null)
                {
                    scanList.Items.Remove(selectedItem);
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
                else
                {
                    MessageBox.Show("Selected channel not found in the scan list.", "Error");
                }
            }
        }
    }
}
