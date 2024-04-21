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
        private static string appVersion = "R01.12.00";
        private AppType currentAppType = AppType.CPS;
        private bool isSysKeyPresent = false;

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
            bool isBornSysIDEnabled = false;

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
                    isBornSysIDEnabled = true;
                    break;
                case "Labtool":
                case "PhpSplutions":
                    // For Labtool and PhpSplutions, all fields are enabled
                    MessageBox.Show("Lab mode enabled!");
                    isSerialNumberEnabled = true;
                    isModelEnabled = true;
                    isCodeplugVersionEnabled = true;
                    isLastProgramSrcEnabled = true;
                    isBornSysIDEnabled = true;
                    break;
            }

            serialNumberBox.Enabled = isSerialNumberEnabled;
            modelBox.Enabled = isModelEnabled;
            codeplugVersionBox.Enabled = isCodeplugVersionEnabled;
            lastProgramSrcBox.Enabled = isLastProgramSrcEnabled;
            txtBornSysID.Enabled = isBornSysIDEnabled;
        }

        private void LoadCodeplug(string json)
        {
            string oldSysId = string.Empty;

            if (currentCodeplug != null)
            {
                oldSysId = currentCodeplug.HomeSystemId;
            }

            currentCodeplug = JsonConvert.DeserializeObject<Codeplug>(json);

            if (!string.IsNullOrEmpty(oldSysId))
            {
                if (oldSysId != currentCodeplug.HomeSystemId)
                {
                    isSysKeyPresent = false;
                    UpdateSysKeyView();
                    MessageBox.Show("Load the valid sys key for this system!");
                }
            }

            if (currentCodeplug == null)
                return;

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
                    var text = !string.IsNullOrEmpty(channel.Tgid) ? channel.Tgid : FormatFrequency(channel.Frequency);
                    zoneNode.Nodes.Add(new TreeNode($"{channel.Alias} ({text})"));
                }
                zonesParentNode.Nodes.Add(zoneNode);
            }

            zonesParentNode.Expand();

            TreeNode scanListsParentNode = new TreeNode("Scan Lists");
            treeView1.Nodes.Add(scanListsParentNode);
            if (treeView1.SelectedNode == scanListsParentNode)

            {
                tabControl1.SelectedTab = tabPage3;
            }
            else if (treeView1.SelectedNode == zonesParentNode)
            {
               
            }

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
            PopulateControlHeadComboBox();
            PopulateRadioModeComboBox();

            serialNumberBox.Text = currentCodeplug.SerialNumber;
            modelBox.Text = currentCodeplug.ModelNumber;
            codeplugVersionBox.Text = currentCodeplug.CodeplugVersion;
            chkSecondaryRadioTx.Checked = currentCodeplug.SecondaryRadioTx;
            chkEnforceSysID.Checked = currentCodeplug.EnforceSystemId;
            chkRequireSysKey.Checked = currentCodeplug.RequireSysKey;
            txtHomeSysID.Text = currentCodeplug.HomeSystemId;
            txtPassword.Text = currentCodeplug.HashedPassword;
            chkCpgPassword.Checked = currentCodeplug.IsPasswordProtected;

            if (string.IsNullOrEmpty(currentCodeplug.BornSystemId))
                currentCodeplug.BornSystemId = currentCodeplug.HomeSystemId;

            txtBornSysID.Text = currentCodeplug.BornSystemId;

            UpdateSysKeyView();

            if (currentCodeplug.RadioMode == 1)
                chkSecondaryRadioTx.Enabled = true;
            else
                chkSecondaryRadioTx.Enabled = false;

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

                    if (CheckIfProtected(fileContent))
                    {
                        using (PasswordForm passwordForm = new PasswordForm())
                        {
                            if (passwordForm.ShowDialog() == DialogResult.OK)
                            {
                                fileContent = CryptoUtils.DecryptString(fileContent, passwordForm.Password);
                                LoadCodeplug(fileContent);
                                saveCodeplugRibbonButton.Enabled = true;
                                saveRibbonQButton.Enabled = true;
                                saveStartMenu.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("Password is required to load this codeplug.", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        LoadCodeplug(fileContent);
                        saveCodeplugRibbonButton.Enabled = true;
                        saveRibbonQButton.Enabled = true;
                        saveStartMenu.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool CheckIfProtected(string fileContent)
        {
            try
            {
                var jsonObject = Newtonsoft.Json.Linq.JToken.Parse(fileContent);
                return false;
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error Parsing JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            txtZoneName.Clear();
            txtChannelName.Clear();
            txtTgid.Clear();
            txtChannelFrequncy.Clear();
            txtScanListName.Clear();

            if (e.Node.Level == 1 && e.Node.Parent != null && e.Node.Parent.Text == "Zones")
            {
                txtZoneName.Text = e.Node.Text;
                Zone selectedZone = currentCodeplug.Zones.FirstOrDefault(z => z.Name == e.Node.Text);
                tabControl1.SelectedTab = tabPage1;
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
                    string identifier = parts[1].TrimEnd(')');

                    Channel selectedChannel = currentCodeplug.Zones
                        .SelectMany(z => z.Channels)
                        .FirstOrDefault(c => c.Alias == txtChannelName.Text &&
                            (c.Tgid == identifier || FormatFrequency(c.Frequency) == identifier));
                    tabControl1.SelectedTab = tabPage2;
                    if (selectedChannel != null)
                    {
                        txtTgid.Text = selectedChannel.Tgid;
                        txtChannelFrequncy.Text = FormatFrequency(selectedChannel.Frequency);
                        PopulateChannelModeComboBox(selectedChannel);
                        if (!requireSysKey())
                            cmbChannelMode.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Channel details could not be found.", "Error");
                    }
                }
            }
            else if (e.Node.Level == 1 && e.Node.Parent != null && e.Node.Parent.Text == "Scan Lists")
            {
                tabControl1.SelectedTab = tabPage3;
                txtScanListName.Text = e.Node.Text;
                var scanList = currentCodeplug.ScanLists.FirstOrDefault(sl => sl.Name == e.Node.Text);
                if (scanList != null)
                {
                    var channels = currentCodeplug.Zones.SelectMany(z => z.Channels).ToList();
                    PopulateChannelsComboBox(channels, scanList);
                }
            }
        }

        private void PopulateChannelsComboBox(List<Channel> channels, ScanList currentScanList)
        {
            cmbChannels.Items.Clear();
            var existingTgids = new HashSet<string>(currentScanList.Items.Select(item => item.Tgid));

            foreach (var channel in channels)
            {
                if (!existingTgids.Contains(channel.Tgid))
                {
                    var text = String.Empty;
                    if (!string.IsNullOrEmpty(channel.Tgid))
                        text = channel.Tgid;
                    else
                        text = FormatFrequency(channel.Frequency);

                    cmbChannels.Items.Add($"{channel.Alias} ({text})");
                }
            }

            if (cmbChannels.Items.Count > 0)
                cmbChannels.SelectedIndex = 0;
            else
                cmbChannels.Text = "No available channels";
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

        private void PopulateControlHeadComboBox()
        {
            var values = Enum.GetValues(typeof(ControlHeadType));
            var i = -1; // Probably a crappy way to do it, but meh

            cmbControlHead.Items.Clear();
            cmbControlHead.Items.Add("<None>");

            foreach (var controlHead in values)
            {
                i++;
                if (i == 0)
                    continue;

                cmbControlHead.Items.Add(controlHead);
            }

            cmbControlHead.SelectedIndex = currentCodeplug.ControlHead;
        }

        private void PopulateRadioModeComboBox()
        {
            var values = Enum.GetValues(typeof(RadioMode));
            cmbRadioMode.Items.Clear();

            foreach (var mode in values)
            {
                cmbRadioMode.Items.Add(mode);
            }

            cmbRadioMode.SelectedIndex = currentCodeplug.RadioMode;
        }

        private void PopulateChannelModeComboBox(Channel selectedChannel)
        {
            cmbChannelMode.Items.Clear();
            foreach (ChannelMode mode in Enum.GetValues(typeof(ChannelMode)))
            {
                cmbChannelMode.Items.Add(mode);
            }
            cmbChannelMode.SelectedItem = selectedChannel.Mode;

            PopulateMode(selectedChannel);
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

        private string GetChannelDisplayText(Channel channel)
        {
            return !string.IsNullOrEmpty(channel.Tgid) ? $"{channel.Alias} ({channel.Tgid})" : $"{channel.Alias} ({FormatFrequency(channel.Frequency)})";
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
                    var channelToSave = selectedZone.Channels.FirstOrDefault(c => GetChannelDisplayText(c) == selectedChannelNode.Text);
                    if (channelToSave != null)
                    {
                        var newText = !string.IsNullOrEmpty(txtTgid.Text) ? txtTgid.Text : txtChannelFrequncy.Text;

                        channelToSave.Alias = txtChannelName.Text;
                        channelToSave.Tgid = txtTgid.Text;
                        channelToSave.Frequency = ParseFrequency(txtChannelFrequncy.Text);
                        channelToSave.Mode = (ChannelMode)cmbChannelMode.SelectedIndex;

                        selectedChannelNode.Text = GetChannelDisplayText(channelToSave);
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
            currentCodeplug.ControlHead = cmbControlHead.SelectedIndex;
            currentCodeplug.RadioMode = cmbRadioMode.SelectedIndex;
            currentCodeplug.RequireSysKey = chkRequireSysKey.Checked;
            currentCodeplug.EnforceSystemId = chkEnforceSysID.Checked;
            currentCodeplug.HomeSystemId = txtHomeSysID.Text;
            currentCodeplug.BornSystemId = txtBornSysID.Text;
            currentCodeplug.HashedPassword = txtPassword.Text;
            currentCodeplug.IsPasswordProtected = chkCpgPassword.Checked;

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
                string json = JsonConvert.SerializeObject(currentCodeplug, Formatting.Indented);

                if (currentCodeplug.IsPasswordProtected && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    json = CryptoUtils.EncryptString(json, txtPassword.Text);
                }

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
                    if (currentCodeplug.ScanLists.Any(sl => sl.Name.Equals(newName, StringComparison.OrdinalIgnoreCase) && sl != scanList))
                    {
                        MessageBox.Show("Another scan list with this name already exists.", "Error");
                        return;
                    }

                    scanList.Name = newName;
                    treeView1.SelectedNode.Text = newName;

                    MessageBox.Show("Scan list name updated successfully.", "Success");
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
            if (treeView1.SelectedNode?.Tag is ScanList scanList && cmbChannels.SelectedItem != null)
            {
                string selectedItem = cmbChannels.SelectedItem.ToString();
                var parts = selectedItem.Split(new string[] { " (" }, StringSplitOptions.None);
                string alias = parts[0].Trim();
                string identifier = parts[1].TrimEnd(')');

                Channel channel = currentCodeplug.Zones
                    .SelectMany(z => z.Channels)
                    .FirstOrDefault(c => c.Alias == alias && (c.Tgid == identifier || FormatFrequency(c.Frequency) == identifier));

                if (channel != null)
                {
                    ScanListItem newItem = new ScanListItem
                    {
                        Alias = channel.Alias,
                        Tgid = channel.Tgid,
                        Frequency = channel.Frequency,
                        Mode = channel.Mode
                    };

                    scanList.Items.Add(newItem);

                    string displayText = $"{newItem.Alias} ({(string.IsNullOrEmpty(newItem.Tgid) ? FormatFrequency(newItem.Frequency) : newItem.Tgid)})";
                    TreeNode newNode = new TreeNode(displayText);
                    treeView1.SelectedNode.Nodes.Add(newNode);
                    treeView1.SelectedNode.Expand();
                }
                else
                {
                    MessageBox.Show("Channel not found.", "Error");
                }

                PopulateChannelsComboBox(currentCodeplug.Zones.SelectMany(z => z.Channels).ToList(), scanList);
            }
            else
            {
                MessageBox.Show("Please select a scan list and a channel to add.", "Error");
            }
        }


        private void btnRemoveChannelFromScanList_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Parent?.Tag is ScanList scanList)
            {
                string fullText = treeView1.SelectedNode.Text;
                int startIndex = fullText.IndexOf('(') + 1;
                int endIndex = fullText.IndexOf(')');
                if (startIndex < 0 || endIndex < 0 || endIndex <= startIndex)
                {
                    MessageBox.Show("Failed to parse the channel information.", "Error");
                    return;
                }

                string identifier = fullText.Substring(startIndex, endIndex - startIndex).Trim();

                bool isFrequency = identifier.All(char.IsDigit) && identifier.Length > 5;

                var selectedItem = scanList.Items.FirstOrDefault(i =>
                    (isFrequency ? FormatFrequency(i.Frequency) == identifier : i.Tgid == identifier) && i.Alias == fullText.Substring(0, startIndex - 2).Trim());

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

        private void btnAddScanList_Click(object sender, EventArgs e)
        {
            string newScanListName = "new list";
            if (!string.IsNullOrWhiteSpace(newScanListName))
            {
                if (!currentCodeplug.ScanLists.Any(sl => sl.Name.Equals(newScanListName, StringComparison.OrdinalIgnoreCase)))
                {
                    ScanList newScanList = new ScanList { Name = newScanListName };
                    currentCodeplug.ScanLists.Add(newScanList);
                    TreeNode newNode = new TreeNode(newScanList.Name) { Tag = newScanList };
                    TreeNode parentNode = treeView1.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == "Scan Lists");
                    parentNode?.Nodes.Add(newNode);
                    parentNode?.Expand();
                }
                else
                {
                    MessageBox.Show("A scan list with this name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Scan list name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteScanList_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null &&
                treeView1.SelectedNode.Parent.Text == "Scan Lists" && treeView1.SelectedNode.Tag is ScanList scanList)
            {
                if (MessageBox.Show("Are you sure you want to delete this scan list?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    currentCodeplug.ScanLists.Remove(scanList);
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
            else
            {
                MessageBox.Show("Please select a scan list to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cmbRadioMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkSecondaryRadioTx.Checked = currentCodeplug.SecondaryRadioTx;
            currentCodeplug.RadioMode = cmbRadioMode.SelectedIndex;

            if (currentCodeplug.RadioMode == 1)
            {
                chkSecondaryRadioTx.Enabled = true;
            } else
            {
                chkSecondaryRadioTx.Enabled = false;
            }
        }

        private void chkSecondaryRadioTx_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PopulateMode(Channel selectedChannel)
        {
            if (selectedChannel == null)
            {
                // TODO: Weird and off that this is null when switching between channels with different modes. Just returning seems to work but this seems like a possibly bigger issue
                //MessageBox.Show("selectedChannel was null BUGBUG");
                return;
            }

            if (selectedChannel.Mode == ChannelMode.P25Conventional || selectedChannel.Mode == ChannelMode.AnalogConventional)
            {
                txtChannelFrequncy.Text = FormatFrequency(selectedChannel.Frequency);
                txtTgid.Text = string.Empty;
                txtTgid.Enabled = false;

                if (!requireSysKey())
                    txtChannelFrequncy.Enabled = true;
            }
            else
            {
                txtChannelFrequncy.Text = string.Empty;
                txtTgid.Text = selectedChannel.Tgid;
                txtChannelFrequncy.Enabled = false;

                if (!requireSysKey())
                    txtTgid.Enabled = true;
            }
        }

        private void cmbChannelMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 2)
            {
                var selectedChannel = currentCodeplug.Zones
                    .SelectMany(z => z.Channels)
                    .FirstOrDefault(c => $"{c.Alias} ({c.Tgid})" == treeView1.SelectedNode.Text);

                if (selectedChannel != null)
                {
                    selectedChannel.Mode = (ChannelMode)cmbChannelMode.SelectedItem;
                }

                PopulateMode(selectedChannel);
            }
        }

        private string FormatFrequency(string frequencyHz)
        {
            if (long.TryParse(frequencyHz, out long freq))
            {
                return (freq / 1000000.0).ToString("N3").Replace(",", "");
            }
            return string.Empty;
        }

        private string ParseFrequency(string frequencyMHz)
        {
            string cleanInput = new string(frequencyMHz.Where(c => char.IsDigit(c) || c == '.').ToArray());

            var parts = cleanInput.Split('.');
            string normalizedInput;
            if (parts.Length > 1)
            {
                normalizedInput = parts[0] + "." + string.Join("", parts.Skip(1));
            }
            else
            {
                normalizedInput = parts[0];
            }

            if (double.TryParse(normalizedInput, out double freq))
            {
                long freqInHz = (long)(freq * 1000000);
                return freqInHz.ToString("D9");
            }
            return string.Empty;
        }

        private bool requireSysKey()
        {
            bool result = true;
            if (isSysKeyPresent || !currentCodeplug.RequireSysKey || currentAppType == AppType.Labtool || currentAppType == AppType.Depot)
                result = false;
            else if (currentCodeplug.RequireSysKey)
                result = true;

            return result;
        }

        private void UpdateSysKeyView()
        {
            if (!requireSysKey())
            {
                chkEnforceSysID.Enabled = true;
                chkRequireSysKey.Enabled = true;
                txtChannelName.Enabled = true;
                txtChannelFrequncy.Enabled = true;
                txtHomeSysID.Enabled = true;
                txtTgid.Enabled = true;
                cmbChannelMode.Enabled = true;
                txtIsSyskeyPresent.Text = "True";
            }
            else
            {
                chkEnforceSysID.Enabled = false;
                chkRequireSysKey.Enabled = false;
                txtChannelName.Enabled = false;
                txtChannelFrequncy.Enabled = false;
                txtHomeSysID.Enabled = false;
                txtTgid.Enabled = false;
                cmbChannelMode.Enabled = false;
                txtIsSyskeyPresent.Text = "False";
            }
        }

        private void btnLoadSysKey_Click(object sender, EventArgs e)
        {
            if (currentCodeplug == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Sys Key File",
                Filter = "Key Files|*.key|All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sysId = currentCodeplug.HomeSystemId;
                    SysKey sysKey = new SysKey(sysId);
                    if (sysKey.ValidateKeyFile(openFileDialog.FileName))
                    {
                        isSysKeyPresent = true;
                        UpdateSysKeyView();
                        MessageBox.Show("SysKey is valid for this system ID.", "Validation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        isSysKeyPresent = false;
                        UpdateSysKeyView();
                        MessageBox.Show("SysKey is not valid for this system ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    isSysKeyPresent = false;
                    UpdateSysKeyView();
                    Console.Write($"Error in syskey: {ex}");
                    MessageBox.Show("SysKey is not valid for this system ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void chkCpgPassword_CheckedChanged(object sender, EventArgs e)
        {
            currentCodeplug.IsPasswordProtected = chkCpgPassword.Checked;
        }

        private void readCodeplugRibbonButton_Click(object sender, EventArgs e)
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

                    if (CheckIfProtected(fileContent))
                    {
                        using (PasswordForm passwordForm = new PasswordForm())
                        {
                            if (passwordForm.ShowDialog() == DialogResult.OK)
                            {
                                fileContent = CryptoUtils.DecryptString(fileContent, passwordForm.Password);
                                LoadCodeplug(fileContent);
                                saveCodeplugRibbonButton.Enabled = true;
                                saveRibbonQButton.Enabled = true;
                                saveStartMenu.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("Password is required to load this codeplug.", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        LoadCodeplug(fileContent);
                        saveCodeplugRibbonButton.Enabled = true;
                        saveRibbonQButton.Enabled = true;
                        saveStartMenu.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openRibbonQButton_Click(object sender, EventArgs e)
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

                    if (CheckIfProtected(fileContent))
                    {
                        using (PasswordForm passwordForm = new PasswordForm())
                        {
                            if (passwordForm.ShowDialog() == DialogResult.OK)
                            {
                                fileContent = CryptoUtils.DecryptString(fileContent, passwordForm.Password);
                                LoadCodeplug(fileContent);
                                saveCodeplugRibbonButton.Enabled = true;
                                saveRibbonQButton.Enabled = true;
                                saveStartMenu.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("Password is required to load this codeplug.", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        LoadCodeplug(fileContent);
                        saveCodeplugRibbonButton.Enabled = true;
                        saveRibbonQButton.Enabled = true;
                        saveStartMenu.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveCodeplugRibbonButton_Click(object sender, EventArgs e)
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
            currentCodeplug.ControlHead = cmbControlHead.SelectedIndex;
            currentCodeplug.RadioMode = cmbRadioMode.SelectedIndex;
            currentCodeplug.RequireSysKey = chkRequireSysKey.Checked;
            currentCodeplug.EnforceSystemId = chkEnforceSysID.Checked;
            currentCodeplug.HomeSystemId = txtHomeSysID.Text;
            currentCodeplug.BornSystemId = txtBornSysID.Text;
            currentCodeplug.HashedPassword = txtPassword.Text;
            currentCodeplug.IsPasswordProtected = chkCpgPassword.Checked;

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
                string json = JsonConvert.SerializeObject(currentCodeplug, Formatting.Indented);

                if (currentCodeplug.IsPasswordProtected && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    json = CryptoUtils.EncryptString(json, txtPassword.Text);
                }

                File.WriteAllText(saveFileDialog.FileName, json);

            }
        }

        private void saveRibbonQButton_Click(object sender, EventArgs e)
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
            currentCodeplug.ControlHead = cmbControlHead.SelectedIndex;
            currentCodeplug.RadioMode = cmbRadioMode.SelectedIndex;
            currentCodeplug.RequireSysKey = chkRequireSysKey.Checked;
            currentCodeplug.EnforceSystemId = chkEnforceSysID.Checked;
            currentCodeplug.HomeSystemId = txtHomeSysID.Text;
            currentCodeplug.BornSystemId = txtBornSysID.Text;
            currentCodeplug.HashedPassword = txtPassword.Text;
            currentCodeplug.IsPasswordProtected = chkCpgPassword.Checked;

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
                string json = JsonConvert.SerializeObject(currentCodeplug, Formatting.Indented);

                if (currentCodeplug.IsPasswordProtected && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    json = CryptoUtils.EncryptString(json, txtPassword.Text);
                }

                File.WriteAllText(saveFileDialog.FileName, json);

            }
        }

        private void loadskeyRibbonButton_Click(object sender, EventArgs e)
        {
            if (currentCodeplug == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Sys Key File",
                Filter = "Key Files|*.key|All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sysId = currentCodeplug.HomeSystemId;
                    SysKey sysKey = new SysKey(sysId);
                    if (sysKey.ValidateKeyFile(openFileDialog.FileName))
                    {
                        isSysKeyPresent = true;
                        UpdateSysKeyView();
                        MessageBox.Show("SysKey is valid for this system ID.", "Validation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        isSysKeyPresent = false;
                        UpdateSysKeyView();
                        MessageBox.Show("SysKey is not valid for this system ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    isSysKeyPresent = false;
                    UpdateSysKeyView();
                    Console.Write($"Error in syskey: {ex}");
                    MessageBox.Show("SysKey is not valid for this system ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveStartMenu_Click(object sender, EventArgs e)
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
            currentCodeplug.ControlHead = cmbControlHead.SelectedIndex;
            currentCodeplug.RadioMode = cmbRadioMode.SelectedIndex;
            currentCodeplug.RequireSysKey = chkRequireSysKey.Checked;
            currentCodeplug.EnforceSystemId = chkEnforceSysID.Checked;
            currentCodeplug.HomeSystemId = txtHomeSysID.Text;
            currentCodeplug.BornSystemId = txtBornSysID.Text;
            currentCodeplug.HashedPassword = txtPassword.Text;
            currentCodeplug.IsPasswordProtected = chkCpgPassword.Checked;

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
                string json = JsonConvert.SerializeObject(currentCodeplug, Formatting.Indented);

                if (currentCodeplug.IsPasswordProtected && !string.IsNullOrEmpty(txtPassword.Text))
                {
                    json = CryptoUtils.EncryptString(json, txtPassword.Text);
                }

                File.WriteAllText(saveFileDialog.FileName, json);

            }
        }

        private void openStartMenu_Click(object sender, EventArgs e)
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

                    if (CheckIfProtected(fileContent))
                    {
                        using (PasswordForm passwordForm = new PasswordForm())
                        {
                            if (passwordForm.ShowDialog() == DialogResult.OK)
                            {
                                fileContent = CryptoUtils.DecryptString(fileContent, passwordForm.Password);
                                LoadCodeplug(fileContent);
                                saveCodeplugRibbonButton.Enabled = true;
                                saveRibbonQButton.Enabled = true;
                                saveStartMenu.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("Password is required to load this codeplug.", "Password Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        LoadCodeplug(fileContent);
                        saveCodeplugRibbonButton.Enabled = true;
                        saveRibbonQButton.Enabled = true;
                        saveStartMenu.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadskeyStartMenu_Click(object sender, EventArgs e)
        {
            if (currentCodeplug == null)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Sys Key File",
                Filter = "Key Files|*.key|All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sysId = currentCodeplug.HomeSystemId;
                    SysKey sysKey = new SysKey(sysId);
                    if (sysKey.ValidateKeyFile(openFileDialog.FileName))
                    {
                        isSysKeyPresent = true;
                        UpdateSysKeyView();
                        MessageBox.Show("SysKey is valid for this system ID.", "Validation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        isSysKeyPresent = false;
                        UpdateSysKeyView();
                        MessageBox.Show("SysKey is not valid for this system ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    isSysKeyPresent = false;
                    UpdateSysKeyView();
                    Console.Write($"Error in syskey: {ex}");
                    MessageBox.Show("SysKey is not valid for this system ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
