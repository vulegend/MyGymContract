using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGymContract
{
    [System.Serializable]
    public class SyncAction
    {
        public string Operation { get; set; }
        public ContractInformation ObjectValue { get; set; }

        public void Resolve()
        {
            switch(Operation)
            {
                case "INSERT":
                    Database.Instance.InsertContract(ObjectValue);
                    break;
                case "DELETE":
                    Database.Instance.DeleteContract(ObjectValue);
                    break;
                case "MODIFY":
                    Database.Instance.ModifyContract(ObjectValue);
                    break;
            }
        }
    }
}
