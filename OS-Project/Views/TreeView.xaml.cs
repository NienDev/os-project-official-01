using OS_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static OS_Project.Views.TreeView;

namespace OS_Project.Views
{
    /// <summary>
    /// Interaction logic for TreeView.xaml
    /// </summary>
    public partial class TreeView : UserControl
    {
        public class TreeViewContext
        {

            public List<Driver> Drivers { get; set; }

            public TreeViewContext()
            {
                Drivers = new List<Driver>();
            }
        }


        public class NTFS
        {
            const string SystemFileName = "NTFS";
            uint BytesPerSector;
            uint SectorsPerCluster;
            uint SectorsPerTrack;
            uint NumOfHeads;
            uint TotalSectors;
            uint StartingClusterMFT;
            uint StartingClusterMFT_mirror;
            uint BytesPerEntry;

            byte[] buffer = new byte[1024];
            uint MFTStartInByte;

            //MFT Attribute ID
            const int STANDARD_INFORMATION = 0x10;
            const int FILE_NAME = 0x30;
            const int DATA = 0x80;

            const int BITMAP = 0xB0;

            public NTFS()
            {
                BytesPerSector = 0;
                SectorsPerCluster = 0;
                SectorsPerTrack = 0;
                NumOfHeads = 0;
                TotalSectors = 0;
                StartingClusterMFT = 0;
                StartingClusterMFT_mirror = 0;
                BytesPerEntry = 0;
            }
            public NTFS(uint _BytesPerSector, uint _SectorsPerCluster, uint _SectorsPerTrack, uint _NumOfHeads, uint _TotalSectors, uint _StartingClusterMFT, uint _StartingClusterMFT_mirror, uint _BytesPerEntry)
            {
                BytesPerSector = _BytesPerSector;
                SectorsPerCluster = _SectorsPerCluster;
                SectorsPerTrack = _SectorsPerTrack;
                NumOfHeads = _NumOfHeads;
                TotalSectors = _TotalSectors;
                StartingClusterMFT = _StartingClusterMFT;
                StartingClusterMFT_mirror = _StartingClusterMFT_mirror;
                BytesPerEntry = _BytesPerEntry;
            }

            public bool GetBitValue(int num, int i)
            {
                int mask = 1 << i;
                int maskedNum = num & mask;
                return (maskedNum != 0);
            }
            public bool isEmptyEntry(byte[] buffer)
            {
                foreach (byte b in buffer)
                {
                    if (b != 0)
                    {
                        return false; //neu van con la entry
                    }
                }
                return true; // neu kh la entry
            }
            public long calStartingAttribute(byte[] buffer)
            {
                return (long)BitConverter.ToInt16(buffer, 0x14);
            }
            public int calNumbOfEntries(byte[] buffer, long StartingAttribute)
            {
                long AttributeLength = (int)BitConverter.ToInt32(buffer, (int)StartingAttribute + 0x04); // Attribute Length of STANDARD

                //attribute [1]: $FILE NAME
                StartingAttribute += AttributeLength;
                AttributeLength = (int)BitConverter.ToInt32(buffer, (int)StartingAttribute + 0x04);

                //attribuute[2]: $DATA (Non-Resident)
                StartingAttribute += AttributeLength;
                long TotalMFTSize = (long)BitConverter.ToInt64(buffer, (int)StartingAttribute + 0x28);
                return (int)TotalMFTSize / 1024;
            }
            public bool isFolder(byte[] buffer)
            {
                int flag = (int)BitConverter.ToInt16(buffer, 0x16);
                if (flag == 0x01)
                {
              
                    return false;
                }
                if (flag == 0x03)
                {
              
                    return true;
                }
                return true;
            }
            public bool isDelete(byte[] buffer)
            {
                int flag = (int)BitConverter.ToInt16(buffer, 0x16);
                if (flag == 0x00 || flag == 0x02)
                    return true;
                return false;
            }
            public bool isResident(byte[] buffer, int StartingAttribute)
            {
                int temp = (int)buffer[StartingAttribute + 0x08];
                if (temp == 0x00)
                    return true;
                if (temp == 0x01)
                    return false;
                return false;
            }

            public bool isZoneIdentifier(byte[] buffer, int StartingAttribute)
            {
                //string Name = "";
                int NameLength = (int)buffer[StartingAttribute + 0x09];
                //int NameOffset = (int)BitConverter.ToInt16(buffer, StartingAttribute + 0xA);
                if (NameLength != 0)
                {
                    //Name = Encoding.Unicode.GetString(buffer,StartingAttribute + NameOffset, NameLength * 2);
                    return true;
                }
                return false;
            }

            public void StandardInformation_Reader(byte[] buffer, ref int StartingAttribute, ref DateTime CreatedTime, ref DateTime LastModified, ref bool isReadOnly, ref bool isHidden, ref bool isSystem)
            {

                long Datetemp = 0;
                //Created Time
                Datetemp = (long)BitConverter.ToInt64(buffer, (int)StartingAttribute + 0x18);
                CreatedTime = DateTime.FromFileTime(Datetemp).ToLocalTime();

                //Last Modified Time
                Datetemp = (long)BitConverter.ToInt64(buffer, (int)StartingAttribute + 0x18 + 0x08);
                LastModified = DateTime.FromFileTime(Datetemp).ToLocalTime();

                //Check its property
                int PropertyIndex = (int)StartingAttribute + 0x38;
                int Property = (int)BitConverter.ToInt32(buffer, PropertyIndex);
                isReadOnly = GetBitValue(Property, 0);
                isHidden = GetBitValue(Property, 1);
                isSystem = GetBitValue(Property, 2);

                //Update for the next attribute
                int AttributeLength = (int)BitConverter.ToInt32(buffer, (int)StartingAttribute + 0x04);
                StartingAttribute += AttributeLength;
            }
            public void FileName_Reader(byte[] buffer, ref int StartingAttribute, ref int ParentID, ref string FileName)
            {

                // Get name of file
                int FileNameLength = (int)buffer[StartingAttribute + 0x58];
                FileName = Encoding.Unicode.GetString(buffer, StartingAttribute + 0x5A, FileNameLength * 2);

                //get parent id of file
                ParentID = calcuParentID(buffer, StartingAttribute);

                //Update StartingAttribute
                int AttributeLength = (int)BitConverter.ToInt32(buffer, (int)StartingAttribute + 0x04);
                StartingAttribute += AttributeLength;


            }
            public void Data_Reader(byte[] buffer, ref int StartingAttribute, ref long SizeOfFile, ref string Type)
            {
                //Check Resident or non-resident

                if (!isZoneIdentifier(buffer, StartingAttribute))
                {
                    if (isResident(buffer, StartingAttribute))
                    {
                        SizeOfFile = (long)BitConverter.ToInt32(buffer, StartingAttribute + 0x10);
                    }
                    else
                        SizeOfFile = (long)BitConverter.ToInt64(buffer, StartingAttribute + 0x30);
                }
                //if (isResident(buffer, StartingAttribute))
                //    SizeOfFile = (int)BitConverter.ToInt32(buffer, 0x04);
                //else
                //    SizeOfFile = (long)BitConverter.ToInt64(buffer, StartingAttribute + 0x30);

                //Update StartingAttribute
                int AttributeLength = (int)BitConverter.ToInt32(buffer, (int)StartingAttribute + 0x04);
                if (AttributeLength >= 1024)
                {
                    AttributeLength = (int)BitConverter.ToInt16(buffer, (int)StartingAttribute + 0x04);
                    StartingAttribute += AttributeLength;
                }
                else
                    StartingAttribute += AttributeLength;

            }

            
            public bool isError(byte[] buffer)
            {
                string temp = null;
                temp += (char)buffer[0];
                temp += (char)buffer[1];
                temp += (char)buffer[2];
                temp += (char)buffer[3];
                if (temp == "FILE")
                    return false;
                return true;
            }

            public void BitMap_Reader(byte[] buffer, ref int StartingAttribute)
            {

                int AttributeLength = (int)BitConverter.ToInt16(buffer, (int)StartingAttribute + 0x04);
                StartingAttribute += AttributeLength;

            }

            public int CheckEND(byte[] buffer, int StartingAttribute)
            {
                return (int)BitConverter.ToInt32(buffer, StartingAttribute);
            }
            public int calcuEntryID(byte[] buffer)
            {
                return (int)BitConverter.ToInt32(buffer, 0x2C);
            }
            public int calcuParentID(byte[] buffer, int StartingAttribute)
            {
                int ParentIDIndex = StartingAttribute + 0x18;
                byte[] temp = new byte[8] { buffer[ParentIDIndex], buffer[ParentIDIndex + 1], buffer[ParentIDIndex + 2], buffer[ParentIDIndex + 3], buffer[ParentIDIndex + 4], buffer[ParentIDIndex + 5], 0, 0 };
                return (int)BitConverter.ToInt64(temp, 0);
            }
            public List<NodeInfo> File_Reader_NTFS(string DriveName, long StartingPartition)
            {
                List<NodeInfo> res = new List<NodeInfo>();
                NodeInfo rootNode = new NodeInfo();
                rootNode.Index = 5;
                rootNode.ParentIndex = -1;
                res.Add(rootNode);

                DateTime CreatedTime = DateTime.Now;
                DateTime LastModified = DateTime.Now;
                int ParentID = 0;
                bool isReadOnly = false, isHidden = false, isSystem = false;
                string FileName = "";
                long SizeOfFile = -1;
                int EntryID = 0;
                string Type = "";

                long MFTStartInByte = (long)(StartingClusterMFT * SectorsPerCluster * BytesPerSector + StartingPartition * 512);
                long offset = MFTStartInByte;

                using (FileStream fs = new FileStream(DriveName, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Read(buffer, 0, buffer.Length);
                    int count = 0;
                    int StartingAttribute = (int)calStartingAttribute(buffer);
                    int NumOfEntry = calNumbOfEntries(buffer, StartingAttribute);
                    while (count < NumOfEntry)
                    {
                        ParentID = 0;
                        isReadOnly = false;
                        isHidden = false;
                        isSystem = false;
                        FileName = "$";
                        SizeOfFile = -1;
                        Type = "";
                        EntryID = calcuEntryID(buffer);
                        StartingAttribute = (int)calStartingAttribute(buffer);
                        if (!isError(buffer))
                        {

                            if (isDelete(buffer))
                            {
                                count++;
                                offset = MFTStartInByte + count * 1024;
                                fs.Seek(offset, SeekOrigin.Begin);
                                fs.Read(buffer, 0, buffer.Length);
                            }
                            else
                            {
                                if (isFolder(buffer))
                                    Type = "Folder";
                                else
                                    Type = "File";

                                if (!isEmptyEntry(buffer))
                                {

                                    //StartingAttribute = (int)calStartingAttribute(buffer);
                                    EntryID = calcuEntryID(buffer);
                                    while (CheckEND(buffer, StartingAttribute) != -1)
                                    {
                                        if (buffer[StartingAttribute] == STANDARD_INFORMATION)
                                        {
                                            StandardInformation_Reader(buffer, ref StartingAttribute, ref CreatedTime, ref LastModified, ref isReadOnly, ref isHidden, ref isSystem);
                                        }
                                        else if (buffer[StartingAttribute] == FILE_NAME)
                                        {
                                            FileName_Reader(buffer, ref StartingAttribute, ref ParentID, ref FileName);
                                            if (isSystem || FileName.Contains("$"))
                                            {
                                                break;
                                            }
                                        }
                                        else if (buffer[StartingAttribute] == DATA)
                                        {
                                            Data_Reader(buffer, ref StartingAttribute, ref SizeOfFile, ref Type);
                                        }

                                        else
                                        {
                                            int AttributeLength = (int)BitConverter.ToInt32(buffer, (int)StartingAttribute + 0x04);
                                            if (AttributeLength >= 1024)
                                            {
                                                AttributeLength = (int)BitConverter.ToInt16(buffer, (int)StartingAttribute + 0x04);
                                                StartingAttribute += AttributeLength;
                                            }
                                            else
                                                StartingAttribute += AttributeLength;
                                        }
                                    }

                                    if (!isSystem && !FileName.Contains("$"))
                                    {

                                        #region info

                                        NodeInfo node = new NodeInfo();
                                        node.Index = EntryID;
                                        node.ParentIndex = ParentID;
                                        node.isReadOnly = isReadOnly ? "True" : "False";
                                        node.isSystem = isSystem ? "True" : "False";
                                        node.isHidden = isHidden ? "True" : "False";
                                        node.date = CreatedTime.ToString();
                                        node.timeModified = LastModified.ToString();
                                        node.fullpath = FileName;

                                        node.type = Type;
                                        node.size = (ulong)SizeOfFile;
                                        node.isExpanded = false;

                                        res.Add(node);
                                        #endregion

                                    }
                                }
                                else
                                {
                                    count++;
                                    offset = MFTStartInByte + count * 1024;
                                    fs.Seek(offset, SeekOrigin.Begin);
                                    fs.Read(buffer, 0, buffer.Length);
                                }


                                count++;
                                offset = MFTStartInByte + count * 1024;
                                fs.Seek(offset, SeekOrigin.Begin);
                                fs.Read(buffer, 0, buffer.Length);
                            }
                        } else
                        {
                            count++;
                            offset = MFTStartInByte + count * 1024;
                            fs.Seek(offset, SeekOrigin.Begin);
                            fs.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                return res;
            }
            public void print()
            {
                Console.WriteLine("File system: " + SystemFileName); // File system
                Console.WriteLine("Bytes per sector                     : " + BytesPerSector);
                Console.WriteLine("Sectors per cluster                  : " + SectorsPerCluster);
                Console.WriteLine("Sectors per track                    : " + SectorsPerTrack);
                Console.WriteLine("Number of heads                      : " + NumOfHeads);
                Console.WriteLine("Number of sectors                    : " + TotalSectors);
                Console.WriteLine("The starting cluster of MFT          : " + StartingClusterMFT);
                Console.WriteLine("The starting cluster of MFT (back-up): " + StartingClusterMFT_mirror);
                Console.WriteLine("Bytes per entry in MFT               : " + BytesPerEntry);
                Console.WriteLine();
            }

        }

        public class FAT
        {
            public byte[] BS;
            public byte[] RDET;
            public string driveName;
            public ulong BytesPerSector;          //Bytes per sector
            public ulong SectorsPerCluster;       //Sectors per cluster (Sc)
            public ulong ReservedSector;          //Reversed Sectors (Sb) : Số sector trước bảng FAT
            public ulong NumOfFat;                // so bang FAT
            public ulong SizeOfVolume;            // Size of volume(bytes)
            public ulong FatSize;                 // in sectors (Sf)
            public string Version;
            public ulong StartedCluster;
            public ulong starting_RDET;
            public byte[] FAT_table;
            public ulong startingPosition;
            public long max_size;

            public FAT(ulong startingPartitionPosition, string _driveName)
            {
                driveName = _driveName;

                startingPosition = startingPartitionPosition;

                // init MBR, FAT_table, RDET
                BS = new byte[512];

                RDET = new byte[512];

                using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
                {
                    // get BS
                    fs.Seek((long)startingPartitionPosition, SeekOrigin.Begin);
                    fs.Read(BS, 0, BS.Length);

                    // get BS information
                    BytesPerSector = BitConverter.ToUInt16(BS, 0x0B);          //Bytes per sector
                    SectorsPerCluster = (ulong)BS[0x0D];                        //Sectors per cluster (Sc)
                    ReservedSector = BitConverter.ToUInt16(BS, 0x0E);          //Reversed Sectors (Sb) : Số sector trước bảng FATfs.Close();
                    NumOfFat = (ulong)BS[0x10];                                 //Number of FAT tables (Nf)
                    SizeOfVolume = BitConverter.ToUInt32(BS, 0x20);            //Size of volume
                    FatSize = (ulong)BitConverter.ToInt32(BS, 0x24) * 512;                 //FAT size in sectors (Sf)
                 
                    
                    Version = Encoding.ASCII.GetString(BS, 0x52, 8);         //Version of FAT
                    StartedCluster = (ulong)BitConverter.ToInt32(BS, 0x2C);

                    starting_RDET = startingPartitionPosition + ReservedSector * 512 + FatSize * 2 * 512; // starting position of RDET

                    // get RDET
                    fs.Seek((long)(starting_RDET), SeekOrigin.Begin);
                    fs.Read(RDET, 0, RDET.Length);

                    //get FAT
                    FAT_table = new byte[FatSize];
                    fs.Seek((long)startingPosition + (long)ReservedSector * 512, SeekOrigin.Begin);
                    fs.Read(FAT_table, 0, FAT_table.Length);

                    fs.Close();
                }
            }
        }

        static void printBytes(byte[] arr)
        {
            int i = 1;
            foreach (byte b in arr)
            {
                Console.Write(b.ToString("X2") + " ");
                if (i % 16 == 0)
                {
                    Console.WriteLine();
                    i = 0;
                }
                i++;

            }
        }

        public class Driver
        {
            public string fullname { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public ulong starting_position { get; set; }
        }

        

        public class NodeInfo : ObservableObject
        {
            public string fullpath { get; set; }
            public ulong RDET_start { get; set; }
            public ulong sub_dir_start { get; set; }
            public bool isFile { get; set; }
            private bool _isExpanded;
            public bool isExpanded { get; set; }
            public ulong size { get; set; }
            public string date { get; set; }
            public string time { get; set; }
            public string timeModified { get; set; }
            public string isArchive { get; set; }
            public string isDirectory { get; set; }
            public string isHidden { get; set; }
            public string isSystem { get; set; }
            public string isVolLabel { get; set; }
            public string isReadOnly { get; set; }
            public int Index { get; set; }
            public int ParentIndex { get; set; }
            public string type { get; set; }
            public int sectorPerCluster { get; set; }

            public NodeInfo() {}

            public NodeInfo(NodeInfo _a)
            {
                fullpath = _a.fullpath;
                RDET_start = _a.RDET_start;
                sub_dir_start = _a.sub_dir_start;
                isFile = _a.isFile;
                isExpanded = _a.isExpanded;
                size = _a.size;
                date = _a.date;
                time = _a.time;
                timeModified = _a.timeModified;
                isArchive = _a.isArchive;
                isDirectory = _a.isDirectory;
                isHidden = _a.isHidden;
                isSystem = _a.isSystem;
                isVolLabel = _a.isVolLabel;
                isReadOnly = _a.isReadOnly;
                type = _a.type;
                sectorPerCluster = _a.sectorPerCluster;
                if (!string.IsNullOrEmpty(type)) isFile = (type == "File") ? true : false;
                Index = _a.Index;
                ParentIndex=_a.ParentIndex;

                OnPropertyChanged("Tag");
            }
        }

        public static string PATH = @"\\.\PhysicalDrive1";

        public TreeView()
        {
            InitializeComponent();



            TreeViewContext treeViewContext = new TreeViewContext();

            ulong[] startingPartitionsPosition = getPartitionsStartingPosition();

         
            #region with each partition -> get it's name and format
            int index = 0;

            List<Driver> drivers = new List<Driver>();

            
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    byte[] buffer = new byte[512];
                    using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
                    {
                        fs.Seek((long)startingPartitionsPosition[index], SeekOrigin.Begin);
                        fs.Read(buffer, 0, 512);
                        fs.Close();
                    }

                    string format = Encoding.ASCII.GetString(buffer, 0x03, 4);

                    Driver newDriver = new Driver()
                    {
                        fullname = "Partition " + index.ToString(),
                        name = drive.Name,
                        type = format != "NTFS" ? "FAT32" : format,
                        starting_position = startingPartitionsPosition[index]
                    };

                    index++;

                    drivers.Add(newDriver);
                }
            }

            foreach (Driver driver in drivers)
            {
                treeViewContext.Drivers.Add(driver);
            }
            DataContext = treeViewContext;

            #endregion

        }

        static byte[] getMBR()
        {
            byte[] MBR_buffer = new byte[512];

            using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            {
                fs.Read(MBR_buffer, 0, MBR_buffer.Length);
                fs.Close();
            }
            return MBR_buffer;
        }

        static ulong[] getPartitionsStartingPosition()
        {
            byte[] MBR_buffer = new byte[512];
            MBR_buffer = getMBR();
            ulong[] result = new ulong[4];

            for (int i = 0; i < 4; i++)
            {
                int offset = 0x01BE + (i * 16);
                result[i] = (ulong)BitConverter.ToInt32(MBR_buffer, offset + 8) * 512; // get starting sector and turn to bytes
            }
            return result;
        }

        public List<long> read_FAT(FAT fat)
        {
            List<long> clusters = new List<long>();

            for (long i = 0; i < fat.FAT_table.Length; i += 4)
            {
                clusters.Add(BitConverter.ToInt32(fat.FAT_table, (int)i));
            }

            return clusters;
        }

        private void DriveBtn_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            Driver info = (Driver)clickedButton.Tag;
           
            FolderView.Items.Clear();
            

            if (info.type == "NTFS")
            {

                    byte[] buffer = new byte[512];
                    using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
                    {
                        fs.Seek((long)info.starting_position, SeekOrigin.Begin);
                        fs.Read(buffer, 0, buffer.Length);
                    }
                    NTFS newNTFS = NTFS_VBR(buffer);

                    List<NodeInfo> input = newNTFS.File_Reader_NTFS(PATH, (long)info.starting_position / 512);
                    Node root = ConstructTree(input);
               
                    displayNTFSTree(root, null, info.name);
            } else
            {
          
                FAT fat = new FAT(info.starting_position, info.name);
              
                Node root = new Node();

                List<long> clusters = read_FAT(fat);

                FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read);

                
                getFATFileFolderNames((int)fat.StartedCluster,ref clusters, ref fat, ref fs, ref root);

                displayFATTree(root, null, info.name);
            }
        }

       
        public void displayNTFSTree(Node node, TreeViewItem item, string path)
        {
            foreach(Node child in node.children)
            {
                //MessageBox.Show(child.info.Index.ToString());
                var sub_item = new TreeViewItem();
                sub_item.Header = GetFileFolderName(child.info.fullpath);
                child.info.fullpath = path + '\\' + child.info.fullpath;
                sub_item.DataContext = path + '\\' + child.info.fullpath;
                sub_item.Tag = new Node(child.info, child.children);
                sub_item.DataContext = child.info.fullpath;
                sub_item.MouseDoubleClick += NTFS_TreeItem_DoubleClicked;

                if (child.children.Count > 0)
                {
                    sub_item.Items.Add(null);
                    sub_item.Expanded += NTFS_Folder_Expanded;
                    sub_item.Collapsed += NTFS_Folder_Collapsed;
                    sub_item.MouseDoubleClick -= NTFS_TreeItem_DoubleClicked;
                }

                if (item == null)
                {

                    FolderView.Items.Add(sub_item);
                }
                else
                {
                    item.Items.Add(sub_item);
                }
            }
        }

        public void displayFATTree(Node node, TreeViewItem item, string path)
        {
            foreach (Node child in node.children)
            {
                //MessageBox.Show(child.info.Index.ToString());
                var sub_item = new TreeViewItem();
                sub_item.Header = GetFileFolderName(child.info.fullpath);
                child.info.fullpath = path + '\\' + child.info.fullpath;
                sub_item.DataContext = path + '\\' + child.info.fullpath;
                sub_item.Tag = new Node(child.info, child.children);
                sub_item.DataContext = child.info.fullpath;
                sub_item.MouseDoubleClick += TreeItem_DoubleClicked;

                if (child.children.Count > 0)
                {
                    sub_item.Items.Add(null);
                    sub_item.Expanded += Folder_Expanded;
                    sub_item.Collapsed += Folder_Collapsed;
                    sub_item.MouseDoubleClick -= TreeItem_DoubleClicked;
                }

                if (item == null)
                {

                    FolderView.Items.Add(sub_item);
                }
                else
                {
                    item.Items.Add(sub_item);
                }
            }
        }

        private void NTFS_Folder_Expanded(object sender, RoutedEventArgs e)
        {
            if (!e.Handled)
            {
                #region Get current TreeViewItem and its data
                TreeViewItem item = (TreeViewItem)sender;
                Node node = (Node)item.Tag;

                #endregion

                //change the icon of folder from close to expanded
                node.info.isExpanded = true;
                item.Tag = new Node(node.info, node.children);

                #region Display detail info

                FName.Text = GetFileFolderName(node.info.fullpath);
                FSize.Text = sizeUnit(getSize(node)) + "(" + getSize(node) + " bytes)";
                FDate.Text = node.info.date.Remove(node.info.date.IndexOf(" "), node.info.date.Length - node.info.date.IndexOf(" "));
                FTime.Text = node.info.date.Substring(node.info.date.IndexOf(" "), node.info.date.Length - node.info.date.IndexOf(" "));
                FHidden.Text = node.info.isHidden == "True" ? "✅" : "◻";
                FSystem.Text = node.info.isSystem == "True" ? "✅" : "◻️";
                FReadOnly.Text = node.info.isReadOnly == "True" ? "✅" : "◻️";
                FTimeModified.Text = node.info.timeModified;
                FArchive.Text = node.info.isFile ? "✅" : "◻️"; ;
                FDirectory.Text = node.info.isFile ? "◻️" : "✅"; ;
                #endregion

                e.Handled = true;

                #region check if the list contain only dummy data and if yes clear it
                if (item.Items.Count != 1 || item.Items[0] != null) return;

                item.Items.Clear();
                #endregion


                displayNTFSTree(node, item, node.info.fullpath);
            }
        }

        private void NTFS_Folder_Collapsed(object sender, RoutedEventArgs e)
        {

            if (!e.Handled)
            {
                #region Get current TreeViewItem and its data
                TreeViewItem item = (TreeViewItem)sender;
                Node node = (Node)item.Tag;

                #endregion

                //change the icon of folder from close to expanded
                node.info.isExpanded = false;
                item.Tag = new Node(node.info, node.children);

                #region Display detail info

                FName.Text = GetFileFolderName(node.info.fullpath);
                FSize.Text = sizeUnit(getSize(node)) + "(" + getSize(node) + " bytes)";
                FDate.Text = node.info.date.Remove(node.info.date.IndexOf(" "), node.info.date.Length - node.info.date.IndexOf(" "));
                FTime.Text = node.info.date.Substring(node.info.date.IndexOf(" "), node.info.date.Length - node.info.date.IndexOf(" "));
                FHidden.Text = node.info.isHidden == "True" ? "✅" : "◻";
                FSystem.Text = node.info.isSystem == "True" ? "✅" : "◻️";
                FReadOnly.Text = node.info.isReadOnly == "True" ? "✅" : "◻️";
                FTimeModified.Text = node.info.timeModified;
                FArchive.Text = node.info.isFile ? "✅" : "◻️"; ;
                FDirectory.Text = node.info.isFile ? "◻️" : "✅"; ;
                #endregion

                e.Handled = true;
            }
        }



        public NTFS NTFS_VBR(byte[] buffer)
        {
            uint BytesPerSector = (uint)BitConverter.ToUInt16(buffer, 0x0B);      //bytes per sector
            uint SectorsPerCluster = (uint)buffer[0x0D];                        //sectors per cluster
            uint SectorsPerTrack = BitConverter.ToUInt16(buffer, 0x18);         //Sectors per track
            uint NumOfHeads = BitConverter.ToUInt16(buffer, 0x1A);                    //Number of heads
            uint TotalSectors = (uint)BitConverter.ToUInt64(buffer, 0x28);            //Number of sectors
            uint StartingClusterMFT = (uint)BitConverter.ToUInt64(buffer, 0x30);   //The starting cluster of MFT
            uint StartingCLusterMFT_mirror = (uint)BitConverter.ToUInt64(buffer, 0x30);   //The starting cluster of MFT (back-up)
            uint BytesPerEntry_temp = (uint)buffer[0x40];
            BytesPerEntry_temp = (~BytesPerEntry_temp + 1) & 0xFF;
            long BytesPerEntry = (long)Math.Pow(2, Math.Abs(BytesPerEntry_temp));  //Bytes per entry in MFT 2^|bu2|
            NTFS newNTFS = new NTFS((uint)BytesPerSector, SectorsPerCluster, SectorsPerTrack, NumOfHeads, TotalSectors, StartingClusterMFT, StartingCLusterMFT_mirror, (uint)BytesPerEntry);

            //newNTFS.print();
            return newNTFS;
        }

        public static string GetFileFolderName(string path)
        {
            // if we have empty/null string
            if (string.IsNullOrEmpty(path)) return string.Empty;

            var normalizedPath = path.Replace('/', '\\');

            var lastIndex = normalizedPath.LastIndexOf('\\');

            // if we don't find the backslash index then full path itself is the name
            if (lastIndex <= 0) return path;

            // return the string after the last backslash(the name we want to find)
            return path.Substring(lastIndex + 1);
        }

        static public byte[] getSDET(long starting_cluster, ref List<long> clusters, ref FileStream fs, ref FAT fat)
        {
            long numberOfClusters = 0;
            long temp_starting_cluster = starting_cluster;
            while (starting_cluster != 0x0FFFFFFF && starting_cluster != 0x0FFFFFF7)
            {
                numberOfClusters++;
                starting_cluster = clusters[(int)starting_cluster];
            }

            byte[] sdet = new byte[numberOfClusters * (long)fat.SectorsPerCluster * (long)fat.BytesPerSector];
            starting_cluster = temp_starting_cluster;

            for (long i = 0; i < numberOfClusters; i++)
            {
                long startingClusterPosition = ((int)starting_cluster - 2) * (long)fat.SectorsPerCluster * (long)fat.BytesPerSector + (long)fat.FatSize * 2 + (long)fat.ReservedSector * (long)fat.BytesPerSector + (long)fat.startingPosition;
                fs.Seek(startingClusterPosition, SeekOrigin.Begin);
                fs.Read(sdet, (int)fat.BytesPerSector * (int)fat.SectorsPerCluster * (int)i, (int)fat.BytesPerSector * (int)fat.SectorsPerCluster);
                starting_cluster = clusters[(int)starting_cluster];
            }
            return sdet;
        }

        static public void getFATFileFolderNames(long starting_cluster, ref List<long> clusters, ref FAT fat, ref FileStream fs, ref Node node)
        {
            if (starting_cluster >= clusters.Count) return;
            byte[] sdet = getSDET(starting_cluster, ref clusters, ref fs, ref fat);

            long index = 0;
            string name = "";

            while (true)
            {
                if (index * 32 >= sdet.Length  || sdet[index * 32] == 0)
                {
                    break;
                }
                if (sdet[index * 32] == 0xE5)
                {
                    index++;
                    continue;
                }

                if (sdet[index * 32 + 11] == 0x0F) // if this is long entry
                {
                    name = Encoding.Unicode.GetString(sdet, (int)index * 32 + 1, 10) + Encoding.Unicode.GetString(sdet, (int)index * 32 + 14, 12) + Encoding.Unicode.GetString(sdet, (int)index * 32 + 28, 4) + name;
                }
                else // this a short entry
                {
                    if (name == "") // if there is no long entry, take the name from short entry -> vi tri 00(11 bytes)
                    {
                        if (isDirectory(sdet, (int)index * 32) == "False")
                        { // if entry is file
                            name = Encoding.ASCII.GetString(sdet, (int)index * 32, 8).TrimEnd(' ') + "." + Encoding.ASCII.GetString(sdet, (int)index * 32 + 8, 3);
                        }
                        else
                        {
                            name = Encoding.ASCII.GetString(sdet, (int)index * 32, 8).TrimEnd(' ');
                        }
                    }




                    if (sdet[index * 32 + 11] != 0x16 && sdet[index * 32 + 11] != 0x08 && name != "." && name != "..") // ignore system file 
                    {

                        NodeInfo info = new NodeInfo()
                        {
                            fullpath = name,
                            RDET_start = fat.starting_RDET,
                            sub_dir_start = 0,
                            isFile = true,
                            isExpanded = false,
                            size = getSize(sdet, index * 32),
                            date = getDate(sdet, index * 32),
                            time = getTimeCreated(sdet, index * 32),
                            timeModified = getTimeModified(sdet, index * 32) + " - " + getDateModified(sdet, index * 32),
                            isArchive = isArchive(sdet, index * 32),
                            isDirectory = isDirectory(sdet, index * 32),
                            isHidden = isHidden(sdet, index * 32),
                            isReadOnly = isReadOnly(sdet, index * 32),
                            isSystem = isSystem(sdet, index * 32),
                            isVolLabel = isVolLabel(sdet, index * 32),
                            sectorPerCluster = (int)fat.SectorsPerCluster
                        };

                        node.children.Add(new Node(info));
                        if (isDirectory(sdet, index * 32) == "True") //folder
                        {

                            node.children.Last<Node>().info.isFile = false;
                            long highCluster = BitConverter.ToInt16(sdet, (int)index * 32 + 0x14);
                            long cluster = unchecked((ushort)BitConverter.ToInt16(sdet, ((int)index * 32) + 26));
                            long startingCluster = highCluster << 16 | cluster;


                            Node new_node = new Node();
                            getFATFileFolderNames(startingCluster, ref clusters, ref fat, ref fs, ref new_node);
                            foreach (Node child in new_node.children)
                            {
                                node.children.Last<Node>().children.Add(child);
                            }
                        }

                        Console.WriteLine(name.ToString());
                    }

                    name = "";
                }
                index++;
            }


        }

     

        private void TreeItem_DoubleClicked(object sender, RoutedEventArgs e)
        {

            if (!e.Handled)
            {
                TreeViewItem item = sender as TreeViewItem;
                Node node = (Node)item.Tag;

                #region Display detail info

                FName.Text = GetFileFolderName(node.info.fullpath);
                FSize.Text = sizeUnit(getSize(node)) + "(" + getSize(node) + " bytes)";
                FTime.Text = node.info.time;
                FDate.Text = node.info.date;
                FArchive.Text = node.info.isArchive == "True" ? "✅" : "◻️";
                FHidden.Text = node.info.isHidden == "True" ? "✅" : "◻";
                FSystem.Text = node.info.isSystem == "True" ? "✅" : "◻️";
                FVolLabel.Text = node.info.isVolLabel == "True" ? "✅" : "◻️"; ;
                FDirectory.Text = node.info.isDirectory == "True" ? "✅" : "◻️";
                FReadOnly.Text = node.info.isReadOnly == "True" ? "✅" : "◻️";
                FTimeModified.Text = node.info.timeModified;

                #endregion

                e.Handled = true;
            }
        }

        private void NTFS_TreeItem_DoubleClicked(object sender, RoutedEventArgs e)
        {

            if (!e.Handled)
            {
                TreeViewItem item = sender as TreeViewItem;
                Node node = (Node)item.Tag;

                #region Display detail info

                FName.Text = GetFileFolderName(node.info.fullpath);
                FSize.Text = sizeUnit(getSize(node)) + "(" + getSize(node) + " bytes)";
                FDate.Text = node.info.date.Remove(node.info.date.IndexOf(" "), node.info.date.Length - node.info.date.IndexOf(" "));
                FTime.Text = node.info.date.Substring(node.info.date.IndexOf(" "), node.info.date.Length - node.info.date.IndexOf(" "));
                FHidden.Text = node.info.isHidden == "True" ? "✅" : "◻";
                FSystem.Text = node.info.isSystem == "True" ? "✅" : "◻️";
                FReadOnly.Text = node.info.isReadOnly == "True" ? "✅" : "◻️";
                FTimeModified.Text = node.info.timeModified;
                FArchive.Text = node.info.isFile ? "✅" : "◻️"; ;
                FDirectory.Text = node.info.isFile ? "◻️" : "✅"; ;
                #endregion

                e.Handled = true;
            }
        }

        private bool isAnyFileFolder(ulong startingPosition)
        {

            byte[] data = new byte[512];
            using (FileStream fs = new FileStream(PATH, FileMode.Open, FileAccess.Read))
            {
                fs.Seek((long)(startingPosition), SeekOrigin.Begin); //
                fs.Read(data, 0, data.Length);
                fs.Close();
            }
            if (data[64] == 0) return false;
            return true;
        }

        private void Folder_Collapsed(object sender, RoutedEventArgs e)
        {
          
            if (!e.Handled)
            {
                #region Get current TreeViewItem and its data
                TreeViewItem item = (TreeViewItem)sender;
                Node node = (Node)item.Tag;
                #endregion

                  node.info.isExpanded = false;
                item.Tag = new Node(node.info, node.children);

                #region Display detail info

                FName.Text = GetFileFolderName(node.info.fullpath);
                FSize.Text = sizeUnit(getSize(node)) + "(" + getSize(node) + " bytes)";
                FTime.Text = node.info.time;
                FDate.Text = node.info.date;
                FArchive.Text = node.info.isArchive == "True" ? "✅" : "◻️";
                FHidden.Text = node.info.isHidden == "True" ? "✅" : "◻️";
                FSystem.Text = node.info.isSystem == "True" ? "✅" : "◻️";
                FVolLabel.Text = node.info.isVolLabel == "True" ? "✅" : "◻️"; ;
                FDirectory.Text = node.info.isDirectory == "True" ? "✅" : "◻️";
                FReadOnly.Text = node.info.isReadOnly == "True" ? "✅" : "◻️";
                FTimeModified.Text = node.info.timeModified;

                #endregion

                e.Handled = true;
            }
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
        

            if (!e.Handled)
            {
                #region Get current TreeViewItem and its data
                TreeViewItem item = (TreeViewItem)sender;
                
                Node node = (Node)item.Tag;

                #endregion

                //change the icon of folder from close to expanded
                node.info.isExpanded = true;
                item.Tag = new Node(node.info, node.children);

                #region Display detail info

                FName.Text = GetFileFolderName(node.info.fullpath);
                FSize.Text = sizeUnit(getSize(node)) + "(" + getSize(node) + " bytes)";
                FTime.Text = node.info.time;
                FDate.Text = node.info.date;
                FArchive.Text = node.info.isArchive == "True" ? "✅" : "◻️";
                FHidden.Text = node.info.isHidden == "True" ? "✅" : "◻️";
                FSystem.Text = node.info.isSystem == "True" ? "✅" : "◻";
                FVolLabel.Text = node.info.isVolLabel == "True" ? "✅" : "◻️"; ;
                FDirectory.Text = node.info.isDirectory == "True" ? "✅" : "◻️";
                FReadOnly.Text = node.info.isReadOnly == "True" ? "✅" : "◻️";
                FTimeModified.Text = node.info.timeModified;

                #endregion

                e.Handled = true;

                #region check if the list contain only dummy data and if yes clear it
                if (item.Items.Count != 1 || item.Items[0] != null) return;

                item.Items.Clear();
                #endregion

                displayFATTree(node, item, node.info.fullpath);
                
            }


        }

        #region get File/Folder Detail Infomation
        static ulong getSize(byte[] data, long start)
        {
            ulong size = 0;
            size = (ulong)BitConverter.ToInt32(data, (int)start + 0x1C);
            return size;
        }

        static string getDate(byte[] data, long start)
        {
            string date = "";
            int dec = BitConverter.ToInt16(data, (int)start + 0x10);
            int d = dec & 0x1F;
            dec = dec >> 5;
            int m = dec & 0xF;
            dec = dec >> 4;
            int y = dec + 1980;
            date = d.ToString() + "/" + m.ToString() + "/" + y.ToString();
            return date;
        }

        static string getTimeCreated(byte[] data, long start)
        {
            string time = "";
            byte[] getTime = new byte[4];
            int k = 0;
            for (int i = 0; i < 3; i++)
                getTime[i] = data[start + 0x0D + i];
            getTime[3] = 0x00;
            int dec = BitConverter.ToInt32(getTime, 0);
            int ms = dec & 0x7F;
            dec = dec >> 7;
            int s = dec & 0x3F;
            dec = dec >> 6;
            int m = dec & 0x3F;
            dec = dec >> 6;
            int h = dec;
            time = h.ToString() + ":" + m.ToString() + ":" + s.ToString() + ":" + ms.ToString();
            return time;
        }

        static string getTimeModified(byte[] data, long start)
        {
            string time = "";
            int dec = unchecked((ushort)BitConverter.ToInt16(data, (int)start + 0x16));
            int s = dec & 0x1F;
            s *= 2;
            dec = dec >> 5;
            int m = dec & 0x3F;
            dec = dec >> 6;
            int h = dec;
            TimeSpan a = new TimeSpan(h, m, s);
            time = a.ToString();
            return time;
        }

        #region Attribute
        static string isArchive(byte[] data, long start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x10;
            int check = dec >> 4;
            return check == 1 ? "False" : "True";
        }
        static string isDirectory(byte[] data, long start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x10;
            int check = dec >> 4;
            return check == 1 ? "True" : "False";
        }
        static string isVolLabel(byte[] data, long start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x8;
            int check = dec >> 3;
            return check == 1 ? "True" : "False";
        }
        static string isSystem(byte[] data, long start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x4;
            int check = dec >> 2;
            return check == 1 ? "True" : "False";
        }
        static string isHidden(byte[] data, long start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x2;
            int check = dec >> 1;
            return check == 1 ? "True" : "False";
        }
        static string isReadOnly(byte[] data, long start)
        {
            int dec = (int)data[start + 0x0B];
            dec = dec & 0x1;
            int check = dec;
            return check == 1 ? "True" : "False";
        }
        #endregion

        #endregion

        public class NodeInfoNTFS : ObservableObject
        {
            public int Index { get; set; }
            public int ParentIndex { get; set; }
            public string isReadOnly { get; set; }
            public string isHidden { get; set; }
            public string isSystem { get; set; }
            public string date { get; set; }
            public string dateModified { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public long size { get; set; }
            public bool isExpanded { get; set; }

            public NodeInfoNTFS() { }

            public NodeInfoNTFS(NodeInfoNTFS _a)
            {
                Index = _a.Index; 
                ParentIndex = _a.ParentIndex;
                isReadOnly = _a.isReadOnly; 
                isHidden = _a.isHidden; 
                date = _a.date; 
                dateModified = _a.dateModified; 
                type = _a.type; 
                name = _a.name;
                size = _a.size;
                isSystem = _a.isSystem;
            }
        }

        public class Node
        {
            public NodeInfo info;
            public List<Node> children = new List<Node>();

            public Node() { }
            public Node(NodeInfo _a)
            {
                info = new NodeInfo(_a);
            }
            public Node(NodeInfo _a, List<Node> _children)
            {
                info = new NodeInfo(_a);
                children = _children;
            }
        }

        private Node ConstructTree(List<NodeInfo> input)
        {
            Dictionary<int, Node> nodes = new Dictionary<int, Node> ();
            foreach (NodeInfo data in input)
            {
                nodes[data.Index] = new Node(data);
            }
            foreach (NodeInfo data in input)
            {
                if (nodes.ContainsKey(data.ParentIndex))
                {
                    nodes[data.ParentIndex].children.Add(new Node(data));
                }
            }

            return BuildTree(nodes, 5);
        }

        private Node BuildTree(Dictionary<int, Node> nodes, int nodeID)
        {
            Node node = nodes[nodeID];
            List<Node> children = new List<Node>();
            foreach(Node child in node.children)
            {
                children.Add(BuildTree(nodes, child.info.Index));
            }
            node.children = children;
            return node;
        }

        private ulong getSize(Node node)
        {

            if (node.info.isFile == false)
            {
                ulong sum = 0;
                foreach (Node child in node.children)
                {
                    sum += getSize(child);
                }
                return sum;
            }
            else
            {
                return node.info.size;
            }
        }

        static public string sizeUnit(double size)
        {
            string unit = " ";
            if (size > 1000)
            {
                size /= 1024;
                unit = " K";
            }
            if (size > 1000)
            {
                size /= 1024;
                unit = " M";
            }
            if (size > 1000)
            {
                size /= 1024;
                unit = " G";
            }

            return size.ToString("F2") + unit + "B";

        }

        static string getDateModified(byte[] data, long start)
        {
            string date = "";
            int dec = BitConverter.ToInt16(data, (int)start + 0x18);
            int d = dec & 0x1F;
            dec = dec >> 5;
            int m = dec & 0xF;
            dec = dec >> 4;
            int y = dec + 1980;
            date = d.ToString() + "/" + m.ToString() + "/" + y.ToString();
            return date;
        }
    }
}
