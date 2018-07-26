using Spire.Doc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace MyGymContract
{
    public partial class NewContractForm : Form
    {
        public NewContractForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateContract("Template1.docx");
        }

        private void CreateContract(string template)
        {
            //construct data
            OwnerInformation owner = new OwnerInformation(directorName.Text, directorSurname.Text, directorMail.Text, directorPhone.Text);
            BusinessInformation businessInformation = new BusinessInformation(companyNameTextbox.Text, companyAddress.Text, companyCityTextbox.Text, companyPIBTextbox.Text, companyRegistryTextbox.Text, dinarskiTextbox.Text, devizniTextbox.Text);
            ContractInformation contractInfo = new ContractInformation(contractLengthCombobox.SelectedIndex + 1, packageCombobox.SelectedIndex, owner, businessInformation);
            string contractNumber = DateTime.Now.ToString("MMddyyyymmssf");
            contractInfo.ContractNumber = contractNumber;

            Document doc = new Document();
            doc.LoadFromFile(template);
            doc.Replace("@companyName", contractInfo.GymInformation.Name, true, true);
            doc.Replace("@directorName", contractInfo.GymOwnerInformation.Name, true, true);
            doc.Replace("@directorSurname", contractInfo.GymOwnerInformation.Surname, true, true);
            doc.Replace("@companyCity", contractInfo.GymInformation.City, true, true);
            doc.Replace("@companyAddress", contractInfo.GymInformation.Address, true, true);
            doc.Replace("@PIB", contractInfo.GymInformation.PIB, true, true);
            doc.Replace("@companyRegistryNumber", contractInfo.GymInformation.RegistryNumber, true, true);
            doc.Replace("@dinarski", contractInfo.GymInformation.Dinarski, true, true);
            doc.Replace("@devizni", contractInfo.GymInformation.Devizni, true, true);
            doc.Replace("@directorNumber", contractInfo.GymOwnerInformation.Mobile, true, true);
            doc.Replace("@dateOfSigning", DateTime.Now.ToShortDateString(), true, true);
            doc.Replace("@package", (contractInfo.ContractPackage == 0) ? "Po iskoriscenosti" : "Flat fee", true, true);
            doc.Replace("@duration", contractInfo.ContractLength.ToString(), true, true);
            doc.Replace("@contractNumber", contractInfo.ContractNumber, true, true);
            doc.SaveToFile(contractNumber+".docx");

            ProcessStartInfo info = new ProcessStartInfo(contractNumber + ".docx");
            Process.Start(info);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //construct data
            OwnerInformation owner = new OwnerInformation(directorName.Text, directorSurname.Text, directorMail.Text, directorPhone.Text);
            BusinessInformation businessInformation = new BusinessInformation(companyNameTextbox.Text, companyAddress.Text, companyCityTextbox.Text, companyPIBTextbox.Text, companyRegistryTextbox.Text, dinarskiTextbox.Text, devizniTextbox.Text);
            ContractInformation contractInfo = new ContractInformation(contractLengthCombobox.SelectedIndex + 1, packageCombobox.SelectedIndex, owner, businessInformation);
            string contractNumber = DateTime.Now.ToString("MMddyyyymmssf");
            contractInfo.ContractNumber = contractNumber;
            //upload to the database

            Form1.AllContracts.Add(contractInfo);
            //save locally
            FileStream fs = new FileStream("contracts.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, Form1.AllContracts);
            fs.Close();
            
            if(Database.Instance.CanConnect())
            {
                Database.Instance.InsertContract(contractInfo);
            }
            else
            {
                if (Form1.SyncTree == null)
                    Form1.SyncTree = new List<SyncAction>();

                SyncAction syncAction = new SyncAction { Operation = "INSERT", ObjectValue = contractInfo };
                Form1.SyncTree.Add(syncAction);
                FileStream fsSync = new FileStream("syncdata.syn", FileMode.Create, FileAccess.Write);
                BinaryFormatter bfSync = new BinaryFormatter();
                bfSync.Serialize(fsSync, Form1.SyncTree);
                fsSync.Close();

            }

        }
    }

    [System.Serializable]
    public class ContractInformation
    {
        public int ContractLength { get; set; }
        public int ContractPackage { get; set; }
        public string ContractNumber { get; set; }
        public OwnerInformation GymOwnerInformation { get; set; }
        public BusinessInformation GymInformation { get; set; }
        public DateTime DateSigned { get; set; }

        public ContractInformation(int length, int package, OwnerInformation ownerInfo, BusinessInformation businessInfo)
        {
            ContractLength = length;
            ContractPackage = package;
            GymOwnerInformation = ownerInfo;
            GymInformation = businessInfo;
        }
    }

    [System.Serializable]
    public class OwnerInformation
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public OwnerInformation(string name, string surname, string email, string mobile)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Mobile = mobile;
        }
    }

    [System.Serializable]
    public class BusinessInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PIB { get; set; }
        public string RegistryNumber { get; set; }
        public string Dinarski { get; set; }
        public string Devizni { get; set; }

        public BusinessInformation(string name, string address, string city, string pib, string registryNumber, string dinarski, string devizni)
        {
            Name = name;
            Address = address;
            City = city;
            PIB = pib;
            RegistryNumber = registryNumber;
            Dinarski = dinarski;
            Devizni = devizni;
        }
    }

}
