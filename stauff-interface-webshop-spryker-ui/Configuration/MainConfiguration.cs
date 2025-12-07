using stauff_interface_webshop_spryker_ui.Configuration.Abstract;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace stauff_interface_webshop_spryker_ui.Configuration {
    public enum BoDataServerTypes {
        MSSQL2005 = 4,
        MSSQL2008 = 6,
        MSSQL2012 = 7,
        MSSQL2014 = 8,
        HANADB = 9,
        MSSQL2016 = 10,
        MSSQL2017 = 11,
        MSSQL2019 = 0xF,
        MSSQL2022 = 0x11
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public sealed class CoresuiteXml {
        public string PrintDef { get; set; } = "";
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DossierXml { get; set; } = "";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public sealed class MainConfiguration : CommonMainConfiguration<MainConfiguration> {
        [Category("Webservice - DEBUG")]
        public string PathToTrace { get; set; } = "./trace";

        [Category("SAP Business One - Autre")]
        [DisplayName("Code article pour les frets de transport sur la facture d'acompte")]
        public string ExpenseItemCode { get; set; } = "30000";

        [Category("SAP Business One - Autre")]
        [DisplayName("Code fret")]
        public int ExpenseCode { get; set; }
#if DEBUG
            = 1;
#else
            = 3;
#endif

        [Category("SAP Business One - Autre")]
        [DisplayName("Code transport Express")]
        public int FeightExpress { get; set; }
#if DEBUG
            = 1;
#else
            = 3;
#endif
        [Category("SAP Business One - Autre")]
        [DisplayName("Code transport Standard")]
        public int FeightStandard { get; set; }
#if DEBUG
            = 1;
#else
            = 10;
#endif
        [Category("SAP Business One - Autre")]
        [DisplayName("Code transport ExWorks")]
        public int FeightExWorks { get; set; }
#if DEBUG
            = 1;
#else
            = 20;
#endif
        [Category("SAP Business One - Autre")]
        [DisplayName("Code transport NS")]
        public int FeightNS { get; set; }
#if DEBUG
            = 1;
#else
            = 2;
#endif

        [Category("SAP Business One - Autre")]
        [DisplayName("Code N° de série pour les documents")]
        public int OrderSeries { get; set; } = -1;

        [Category("SAP Business One - Autre")]
        [DisplayName("Compte payment client dummy")]
        public string PaymentAccountDummy { get; set; }
#if DEBUG
            //= "512500";
            = "413101";
#else
            = "419200";
#endif

        [Category("SAP Business One - Autre")]
        [DisplayName("Code méthode de payment client dummy")]
        public string PaymentMethodDummy { get; set; }
#if DEBUG
            //= "Virement";
            = "Vrt Client";
#else
            = "VIRM client";
#endif


        [Category("SAP Business One - Autre")]
        //[DisplayName("Code méthode de payment client dummy")]
        public int PaymentMethodPayPerInvoice { get; set; }
            = 25;

        [Category("SAP Business One - Autre")]
        [DisplayName("Code Client Dummy")]
        public string CodeClientDummy { get; set; }
#if DEBUG
            = "0015I000009NlMO";
#else
            = "C065125";
#endif

        [Category("SAP Business One - Autre")]
        [DisplayName("Code Client Non Soumis à surchage")]
        public string CodeClientNonSoumisASurcharge { get; set; }
#if DEBUG
            = "0015I000009NlMO";
#else 
            = "C065125";
#endif

        [Category("SAP Business One - Autre")]
        [DisplayName("Code Salarié Web")]
        public int CodeSalarie { get; set; }
#if DEBUG
            = 1;
#else
            = 54;
#endif

        [Category("SAP Business One - Autre")]
        [DisplayName("Code Magasin")]
        public string CodeMagasin1 { get; set; }
#if DEBUG
            = "0FR";
#else
            = "01";
#endif
        [Category("SAP Business One - Autre")]
        [DisplayName("Montant Inferieur Auquel Ajout Surcharge")]
        public decimal MontantInferieurAuquelAjoutSurcharge { get; set; }
#if DEBUG
    = 150;
#else
            = 150;
#endif        
        [Category("SAP Business One - Autre")]
        [DisplayName("Article Surcharge")]
        public string ArticleSurcharge { get; set; }
#if DEBUG
    = "20000";
#else
            = "20000";
#endif

        public string VatGroupDownPayment { get; set; } = "C8";

        #region Webservice
        [Category("Webservice")]
        public string ServiceName { get; set; }
#if DEBUG
            = "ERT Interface WebShop Spryker TEST";
#else
            = "ERT Interface WebShop Spryker";
#endif
        [Category("Webservice")]
        public string[] URLs { get; set; } = new string[] {
            "http://*:8080"
        };
        #endregion

        #region SBO
        [Category("SAP Business One - Connexion")]
        [DisplayName("Serveur")]
        [Description("Serveur")]
        public string Server { get; set; }
#if DEBUG
        = "ERT-W10TMA";
#endif
        [Category("SAP Business One - Connexion")]
        [DisplayName("Type de Serveur")]
        [Description("Type de Serveur")]
        public BoDataServerTypes DbServerType { get; set; }
#if DEBUG
        = BoDataServerTypes.MSSQL2019;
#else
        = BoDataServerTypes.MSSQL2022;
#endif
        [Category("SAP Business One - Connexion")]
        [DisplayName("Nom d'utilisateur")]
        [Description("Nom d'utilisateur")]
        public string UserName { get; set; } = "manager";
        [Category("SAP Business One - Connexion")]
        [XmlIgnore]
        [PasswordPropertyText(true)]
        public string Password { get; set; } = "alfamana";

        [Browsable(false)]
        [XmlElement("Password")]
        public string CryptedPassword {
            get {
                try {
                    return Encrypt(Password);
                } catch { return ""; }
            }
            set {
                try {
                    Password = Decrypt(value);
                } catch {
                    Password = value;
                }
            }
        }

        [Category("SAP Business One - Connexion")]
        [DisplayName("Base SBO")]
        [Description("Base SBO")]
        public string CompanyDB { get; set; }
#if DEBUG
        = "ALFA";
#endif
        [Category("SAP Business One - Connexion")]
        [DisplayName("SLD (si besoin)")]
        public string SLDServer { get; set; }
        #endregion

        [Category("Coresuite Pay per Invoice")]
        [DisplayName("Commandes")]
        public CoresuiteXml CoresuiteOrderMail { get; set; } = new CoresuiteXml();

        #region Coresuite PDF
        [Category("Coresuite PDF")]
        [DisplayName("Commandes")]
        public CoresuitePdf CoresuiteOrderPdf { get; set; } = new CoresuitePdf();
        [Category("Coresuite PDF")]
        [DisplayName("Livraison")]
        public CoresuitePdf CoresuiteDeliveryPdf { get; set; } = new CoresuitePdf();
        [Category("Coresuite PDF")]
        [DisplayName("Facture")]
        public CoresuitePdf CoresuiteInvoicePdf { get; set; } = new CoresuitePdf();
        [Category("Coresuite PDF")]
        [DisplayName("Avoir")]
        public CoresuitePdf CoresuiteCreditNotePdf { get; set; } = new CoresuitePdf();
        #endregion

    }

    public enum PdfType {
        CORESUITE,
        CRYSTALREPORTS,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class CoresuitePdf {
        public PdfType Type { get; set; } = PdfType.CORESUITE;
        public string PrintDef { get; set; } = "";
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DossierXml { get; set; } = "";
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DossierPdf { get; set; } = "";
        public string FilenamePattern { get; set; } = "{{DocEntry}}.pdf";
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Rpt { get; set; } = "";
    }
}
