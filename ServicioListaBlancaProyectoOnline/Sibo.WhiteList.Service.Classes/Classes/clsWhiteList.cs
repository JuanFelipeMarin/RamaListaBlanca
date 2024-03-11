using Sibo.WhiteList.Service.BLL;
using Sibo.WhiteList.Service.BLL.Helpers;
using Sibo.WhiteList.Service.Entities.Classes;
using System;

namespace Sibo.WhiteList.Service.Classes
{
    public class clsWhiteList
    {
        public bool GetWhiteList(int gymId, int branchId)
        {
            try
            {
                WhiteListBLL wlBLL = new WhiteListBLL();
                return wlBLL.GetWhiteList(gymId, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidateEntryByUserId(string id, string ipAddress)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                EntryBLL entryBll = new EntryBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponse response = new eResponse();
                string msg = string.Empty;
                rWlEntity = wlBll.ValidateEntryByUserId(id, false);

                if (rWlEntity.response.state)
                {
                    //Validar entradas grupo familiar
                    response = wlBll.ValidateEntriesFamilyGroup(rWlEntity.whiteList, false);

                    if (response.state)
                    {
                        //Validar restricciones horarias
                        response = wlBll.ValidateRestrictions(rWlEntity.whiteList, false);

                        if (response.state)
                        {
                            //Validar minutos no re-ingreso
                            string enableDays = string.Empty;

                            if (rWlEntity.whiteList.availableEntries == 1)
                            {
                                enableDays = "Le queda " + rWlEntity.whiteList.availableEntries.ToString() + " día disponible.";
                            }
                            else
                            {
                                enableDays = "Le quedan " + rWlEntity.whiteList.availableEntries.ToString() + " días disponibles.";
                            }

                            entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                        rWlEntity.whiteList.documentType, false, 0, null, rWlEntity.whiteList.expirationDate, false, "Puede ingresar", rWlEntity.whiteList.planName,
                                        enableDays, true, ipAddress);
                            return true;
                        }
                        else
                        {
                            switch (response.message)
                            {
                                case "NoWL":
                                    msg = "No se pudo obtener la información del usuario que desea ingresar.";
                                    break;
                                case "Ex":
                                    msg = "Ocurró un error en el proceso de validación de restricciones horarias. Error: " + response.additionalMessage;
                                    break;
                                case "NoEntryByRestriction":
                                    msg = "Su plan no contempla esta hora.";
                                    break;
                                default:
                                    msg = "Ocurrió un error en el proceso de validación de las restricciones horarias, por favor vuelva a intentarlo.";
                                    break;
                            }

                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress);
                            return false;
                        }
                    }
                    else
                    {
                        switch (response.message)
                        {
                            case "NoWL":
                                msg = "No se pudo obtener la información del usuario que desea ingresar.";
                                break;
                            case "Ex":
                                msg = "Ocurró un error en el proceso de validación de restricciones horarias. Error: " + response.additionalMessage;
                                break;
                            case "NoEntryByRestriction":
                                msg = "Superado Limite de ingresos del Grupo Familiar.";
                                break;
                            default:
                                msg = "Ocurrió un error en el proceso de validación de las entradas del grupo familiar, por favor vuelva a intentarlo.";
                                break;
                        }

                        clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                        entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress);
                        return false;
                    }
                }
                else
                {
                    switch (rWlEntity.response.message)
                    {
                        case "Ex":
                            msg = "Ocurrió un error en el proceso de consulta en la lista blanca.\nError: " + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        case "NoId":
                            msg = "No es posible validar si el usuario puede ingresar debido a que la identificación no fue ingresada.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "NoEntityWL":
                            msg = "El usuario existe en la lista blanca pero ocurrió un error en el proceso de conversión de los datos (Entidad)." + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        case "NoWhiteList":
                            msg = "El usuario no existe en la lista blanca por lo tanto no es posible su ingreso." + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        default:
                            msg = "Ocurrió un error en el proceso de validación del ingreso del usuario.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                    }

                    entryBll.Insert(clsEnum.EntryType.Entry.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede ingresar", "", msg, false, ipAddress);

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidateExitById(string id, string ipAddress)
        {
            try
            {
                WhiteListBLL wlBll = new WhiteListBLL();
                EntryBLL entryBll = new EntryBLL();
                eResponseWhiteList rWlEntity = new eResponseWhiteList();
                eResponse response = new eResponse();
                eConfiguration config = new eConfiguration();
                AccessControlSettingsBLL acsBll = new AccessControlSettingsBLL();
                string msg = string.Empty;
                bool validateEntryInOutput = false;

                config = acsBll.GetLocalAccessControlSettings();

                if (config != null)
                {
                    validateEntryInOutput = !config.bitNo_Validar_Entrada_En_Salida;
                }

                rWlEntity = wlBll.ValidateExitByUserId(id, false);

                if (rWlEntity.response.state)
                {
                    if (validateEntryInOutput)
                    {
                        entryBll.UpdateExit(id, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId, false);
                        return true;
                    }
                    else
                    {
                        entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, rWlEntity.whiteList.name, rWlEntity.whiteList.planId, rWlEntity.whiteList.invoiceId,
                                        rWlEntity.whiteList.documentType, false, 0, null, null, false, "Puede salir", rWlEntity.whiteList.planName,
                                        "Vuelva pronto.", true, ipAddress);
                        return true;
                    }
                }
                else
                {
                    switch (rWlEntity.response.message)
                    {
                        case "Ex":
                            msg = "Ocurrió un error en el proceso de consulta en la lista blanca.\nError: " + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        case "NoId":
                            msg = "No es posible validar si el usuario puede ingresar debido a que la identificación no fue ingresada.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "NoEntityWL":
                            msg = "El usuario existe en la lista blanca pero ocurrió un error en el proceso de conversión de los datos (Entidad)." + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                        case "NoWhiteList":
                            msg = "El usuario no existe en la lista blanca por lo tanto no es posible su ingreso." + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        case "NoEntry":
                            msg = "El usuario no tiene entrada previa." + rWlEntity.response.additionalMessage;
                            clsShowMessage.Show(msg, clsEnum.MessageType.Informa);
                            break;
                        default:
                            msg = "Ocurrió un error en el proceso de validación de la salida del usuario.";
                            clsShowMessage.Show(msg, clsEnum.MessageType.Error);
                            break;
                    }

                    entryBll.Insert(clsEnum.EntryType.Exit.ToString(), id, "", 0, 0, "", false, 0, null, null, false, "No puede salir", "", msg, false, ipAddress);
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
