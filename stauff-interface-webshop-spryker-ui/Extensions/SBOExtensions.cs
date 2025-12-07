using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.Extensions {
    public static class SBOExtensions {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static void CheckAndCreateUserField(this Company Comp_DI,
            string TableName, string fieldName, string desc,
            int size,
            SAPbobsCOM.BoFieldTypes type = BoFieldTypes.db_Alpha,
            SAPbobsCOM.BoFldSubTypes subType = BoFldSubTypes.st_None,
            Dictionary<string, string> keyValuePairs = null,
            string DefaultValue = null) {

            var dtcheck = (IRecordset)Comp_DI.GetBusinessObject(BoObjectTypes.BoRecordset);
            dtcheck.Query("select \"AliasID\" from \"CUFD\" where \"TableID\"='" + TableName + "' And \"AliasID\"='" + fieldName + "'");

            if(dtcheck.RecordCount <= 0) {
                dtcheck.ReleaseComObject();
                SAPbobsCOM.UserFieldsMD myUserFieldsMD = null;
                try {
                    myUserFieldsMD = (SAPbobsCOM.UserFieldsMD)Comp_DI.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                    myUserFieldsMD.Name = fieldName;
                    myUserFieldsMD.TableName = TableName;
                    myUserFieldsMD.Description = desc;
                    myUserFieldsMD.Type = type;
                    if(type == SAPbobsCOM.BoFieldTypes.db_Alpha || type == SAPbobsCOM.BoFieldTypes.db_Numeric) {
                        if(size > 0 && type == SAPbobsCOM.BoFieldTypes.db_Alpha)
                            myUserFieldsMD.Size = size;
                        myUserFieldsMD.EditSize = size;
                    }
                    myUserFieldsMD.SubType = subType;
                    if(type == BoFieldTypes.db_Alpha && keyValuePairs != null) {
                        foreach(var kv in keyValuePairs) {
                            myUserFieldsMD.ValidValues.Description = kv.Key;
                            myUserFieldsMD.ValidValues.Value = kv.Value;
                            myUserFieldsMD.ValidValues.Add();
                        }
                    }
                    if(!string.IsNullOrWhiteSpace(DefaultValue))
                        myUserFieldsMD.DefaultValue = DefaultValue;

                    int res = myUserFieldsMD.Add();
                    if(res != 0) {
                        throw new Exception(Comp_DI.GetLastErrorDescription());
                    }
                } finally {
                    myUserFieldsMD.ReleaseComObject();
                }
            } else {
                dtcheck.ReleaseComObject();
            }
        }

        public static void Query(this IRecordset rc, string query) {
            query = query.Trim();

            Logger.Debug("Query: " + query);
            rc.DoQuery(query);
            Logger.Debug("RecordCount : " + rc.RecordCount);
        }

        public static object QueryFirstValue(this SAPbobsCOM.ICompany Comp_DI, string query) {
            query = query.Trim();

            Logger.Debug("QueryFirstValue: " + query);
            IRecordset RecCheck = null;
            object rvalue = null;
            try {
                RecCheck = (IRecordset)Comp_DI.GetBusinessObject(BoObjectTypes.BoRecordset);
                RecCheck.DoQuery(query);
                if(RecCheck.RecordCount > 0) {
                    rvalue = RecCheck.Fields.Item(0).Value;
                } else {
                    switch(RecCheck.Fields.Item(0).Type) {
                        case BoFieldTypes.db_Alpha:
                        case BoFieldTypes.db_Memo:
                            rvalue = "";
                            break;
                        case BoFieldTypes.db_Float:
                            rvalue = 0.0d;
                            break;
                        case BoFieldTypes.db_Numeric:
                            rvalue = 0;
                            break;
                        case BoFieldTypes.db_Date:
                            rvalue = "";
                            break;
                        default:
                            rvalue = null;
                            break;
                    }
                }

            } catch(Exception e) {
                Logger.Error("Erreur de requte sur : " + query + "\r\n" + e.ToString());
                rvalue = null;
            } finally {
                if(RecCheck != null)
                    RecCheck.ReleaseComObject();
            }
            Logger.Debug("Return : " + rvalue);
            return rvalue;
        }
        public static IField UserField(this IDocument_Lines line, string field) {
            return line.UserFields.Fields.Item(field);
        }
        public static IField UserField(this IDocuments line, string field) {
            return line.UserFields.Fields.Item(field);
        }
        public static void SetValueCutIfNecessary(this IField field, string value) {
            Logger.Debug("SetValueCutIfNecessary: " + field?.Name + " : " + value);
            if(string.IsNullOrWhiteSpace(value)) return;
            if(field == null) return;
            field.Value = value.CutIfMoreThan(field.Size);
        }
    }
}
