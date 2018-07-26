using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGymContract
{
    public partial class ShowContracts : Form
    {
        public ShowContracts()
        {
            InitializeComponent();
            DisplayDataInGrid();
        }

        public void DisplayDataInGrid()
        {
            listView1.Items.Clear();
            foreach(ContractInformation contract in Form1.AllContracts)
            {
                ListViewItem lViewItem = new ListViewItem(new string[] { contract.ContractNumber, contract.GymInformation.Name, contract.GymInformation.Address, contract.GymInformation.City, contract.GymInformation.PIB, contract.GymInformation.RegistryNumber, contract.GymInformation.Dinarski, contract.GymInformation.Devizni, contract.GymOwnerInformation.Name + " " + contract.GymOwnerInformation.Surname, contract.GymOwnerInformation.Mobile, contract.GymOwnerInformation.Email, contract.ContractLength + " godine", (contract.ContractPackage == 0) ? "Po iskoriscenosti" : "Flat fee", contract.DateSigned.ToShortDateString()});
                listView1.Items.Add(lViewItem);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string contractNumber = textBox1.Text;
            List<ContractInformation> filteredList = Form1.AllContracts.FindAll(x => x.ContractNumber == contractNumber);

            listView1.Items.Clear();
            foreach (ContractInformation contract in Form1.AllContracts)
            {
                ListViewItem lViewItem = new ListViewItem(new string[] { contract.ContractNumber, contract.GymInformation.Name, contract.GymInformation.Address, contract.GymInformation.City, contract.GymInformation.PIB, contract.GymInformation.RegistryNumber, contract.GymInformation.Dinarski, contract.GymInformation.Devizni, contract.GymOwnerInformation.Name + " " + contract.GymOwnerInformation.Surname, contract.GymOwnerInformation.Mobile, contract.GymOwnerInformation.Email, contract.ContractLength + " godine", (contract.ContractPackage == 0) ? "Po iskoriscenosti" : "Flat fee", contract.DateSigned.ToShortDateString() });
                listView1.Items.Add(lViewItem);
            }
        }
    }
}
