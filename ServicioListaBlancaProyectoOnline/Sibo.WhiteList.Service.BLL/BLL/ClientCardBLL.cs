using Sibo.WhiteList.Service.DAL.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.BLL
{
    public class ClientCardBLL
    {
        public int InsertTable(DataTable dt)
        {
            int response = 0;
            dClientCard clientCard = new dClientCard();

            //MToro, verificando si existe alguna tarjeta
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int exist = clientCard.GetCard(Convert.ToString(dt.Rows[i]["clientCardId"]), dt);
                    if (exist > 0)
                        dt.Rows[i].Delete();
                }
            }
            //

            if (dt != null && dt.Rows.Count > 0)
            {
                response = clientCard.InsertTable(dt);
            }

            return response;
        }

        public bool DeleteAll()
        {
            dClientCard clientCard = new dClientCard();
            return clientCard.DeleteAll();
        }

        public bool DeleteByClients(string clients)
        {
            bool resp = false;
            dClientCard clientCard = new dClientCard();

            if (!string.IsNullOrEmpty(clients))
            {
                resp = clientCard.DeleteByClients(clients);
            }

            return resp;
        }

        /// <summary>
        /// Remueve las filas que tengan el parametro colName duplicado MToro
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            DataTable respaldo = dTable; //tabla de respaldo en caso de que ocurra un error
            try
            {
                Hashtable hTable = new Hashtable();
                ArrayList duplicateList = new ArrayList();

                //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
                //And add duplicate item value in arraylist.
                foreach (DataRow drow in dTable.Rows)
                {
                    if (hTable.Contains(drow[colName]))
                        duplicateList.Add(drow);
                    else
                        hTable.Add(drow[colName], string.Empty);
                }

                //Removing a list of duplicate items from datatable.
                foreach (DataRow dRow in duplicateList)
                    dTable.Rows.Remove(dRow);

                //Datatable which contains unique records will be return as output.
                return dTable;
            }
            catch (Exception ex)
            {
                return respaldo;
            }

        }

    }
}
