using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace MyGymContract
{
    public partial class Form1 : Form
    {
        public static List<ContractInformation> AllContracts = new List<ContractInformation>();
        public static List<SyncAction> SyncTree = null;

        public Form1()
        {
            InitializeComponent();
            //try load from cloud
            if (Database.Instance.CanConnect())
                LoadContractsFromCloud();
            else
                LoadContractsLocally();

            //try loading a sync tree if one is left
            GetSyncTree();
            TrySynchronising();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewContractForm newContractForm = new NewContractForm();
            newContractForm.Show();
           
        }

        //Load contracts from cloud and sync locally straight away
        private void LoadContractsFromCloud()
        {
            AllContracts = Database.Instance.GetAllContracts();

            if (AllContracts == null)
                AllContracts = new List<ContractInformation>();
        }

        //Load all contracts locally and create sync tree to sync with the database when connection is available
        private void LoadContractsLocally()
        {
            FileStream fs = new FileStream("contracts.dat", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            AllContracts = (List<ContractInformation>)bf.Deserialize(fs);
            fs.Close();
        }

        private void GetSyncTree()
        {
            if(File.Exists("syncdata.syn"))
            {
                FileStream fs = new FileStream("syncdata.syn", FileMode.Open, FileAccess.Read);
                BinaryFormatter bf = new BinaryFormatter();
                SyncTree = (List<SyncAction>)bf.Deserialize(fs);
                fs.Close();
            }
        }

        private void TrySynchronising()
        {
            if(Database.Instance.CanConnect() && SyncTree != null)
            {
                foreach(SyncAction syncAction in SyncTree)
                {
                    syncAction.Resolve();
                }

                SyncTree = null;
                File.Delete("syncdata.syn");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowContracts showContractsForm = new ShowContracts();
            showContractsForm.Show();
        }
    }
}
