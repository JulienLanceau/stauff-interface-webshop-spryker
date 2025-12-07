using NLog.Targets;
using stauff_interface_webshop_spryker_ui.Configuration;
using stauff_interface_webshop_spryker_ui.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using static ScintillaNET.Style;

namespace stauff_interface_webshop_spryker_ui {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        ScintillaNET.Scintilla TextArea1;
        ScintillaNET.Scintilla TextArea2;
        ScintillaNET.Scintilla TextArea3;
        ScintillaNET.Scintilla TextArea4;
        ScintillaNET.Scintilla TextArea5;
        ScintillaNET.Scintilla TextArea6;
        private void InitScintillaTextArea(ref ScintillaNET.Scintilla ta, ref TabPage tp, ScintillaNET.Lexer lexer = ScintillaNET.Lexer.Json) {
            ta = new ScintillaNET.Scintilla();
            tp.Controls.Add(ta);
            ta.Dock = DockStyle.Fill;
#pragma warning disable CS0618 // Type or member is obsolete
            ta.Lexer = lexer;
#pragma warning restore CS0618 // Type or member is obsolete
            ta.Styles[ScintillaNET.Style.Xml.Tag].ForeColor = System.Drawing.Color.Blue;
            ta.Styles[ScintillaNET.Style.Json.PropertyName].ForeColor = System.Drawing.Color.Blue;
            ta.Margins[0].Width = 25;
            ta.Margins[2].Width = 20;
            ta.Margins[2].Type = ScintillaNET.MarginType.Symbol;
            ta.Margins[2].Mask = ScintillaNET.Marker.MaskFolders;
            ta.Margins[2].Sensitive = true;

            ta.SetProperty("fold", "1");
            ta.SetProperty("fold.compact", "1");
            ta.SetProperty("fold.html", "1");

            ta.Markers[ScintillaNET.Marker.Folder].Symbol = ScintillaNET.MarkerSymbol.BoxPlus;
            ta.Markers[ScintillaNET.Marker.FolderOpen].Symbol = ScintillaNET.MarkerSymbol.BoxMinus;
            ta.Markers[ScintillaNET.Marker.FolderEnd].Symbol = ScintillaNET.MarkerSymbol.BoxPlusConnected;
            ta.Markers[ScintillaNET.Marker.FolderMidTail].Symbol = ScintillaNET.MarkerSymbol.TCorner;
            ta.Markers[ScintillaNET.Marker.FolderOpenMid].Symbol = ScintillaNET.MarkerSymbol.BoxMinusConnected;
            ta.Markers[ScintillaNET.Marker.FolderSub].Symbol = ScintillaNET.MarkerSymbol.VLine;
            ta.Markers[ScintillaNET.Marker.FolderTail].Symbol = ScintillaNET.MarkerSymbol.LCorner;

            ta.AutomaticFold = ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click | ScintillaNET.AutomaticFold.Change;
        }
        private void Form1_Load(object sender, EventArgs e) {
            #region
#if DEBUG
            if(!System.Diagnostics.Debugger.IsAttached) {
#endif
                toolStripButton6.Visible = false;
#if DEBUG
            }
#endif
            #endregion
            propertyGrid1.SelectedObject = MainConfiguration.LoadStatic();
            InitScintillaTextArea(ref TextArea1, ref tabPage4);
            InitScintillaTextArea(ref TextArea2, ref tabPage5, ScintillaNET.Lexer.Xml);
            InitScintillaTextArea(ref TextArea3, ref tabPage6);
            InitScintillaTextArea(ref TextArea4, ref tabPage7, ScintillaNET.Lexer.NotFound);
            InitScintillaTextArea(ref TextArea5, ref tabPage8, ScintillaNET.Lexer.Xml);
            InitScintillaTextArea(ref TextArea6, ref tabPage9, ScintillaNET.Lexer.Xml);

            TextArea1.Text = JsonSerializer.Serialize(new DataContrats.In.Order(), new JsonSerializerOptions {
                WriteIndented = true,
            });
#if DEBUG
            TextArea1.Text = JsonSerializer.Serialize(new DataContrats.In.Order {
                debitorNumber = "",
                vatNumber = "DE305192230",
                customerOrderNumber = "Project #51",
                paymentReference = "ZDKDSNQTZ2RZNN82",
                sprykerOrderNumber = "E-Shop-FR--0815",
                orderComments = "please deliver on tuesday",
                completeDelivery = false,
                freight = new DataContrats.In.Freight {
                    mode = "Express",
                },
                customerAddress = new DataContrats.In.Address {
                    name = "diva-e",
                    name2 = "",
                    name3 = "",
                    name4 = "",
                    street = "Beiertheimer Allee",
                    houseNumber = "18",
                    postalCode = "76137",
                    city = "Karlsruhe",
                    countryCode = "DE",
                    emailAddress = "mail@diva-e.com",
                    telephoneNumber = "0721/9206090",
                },
                shipping = new DataContrats.In.Shipping {
                    partnerNumber = "",
                    address = new DataContrats.In.Address {
                        name = "diva-e",
                        name2 = "",
                        name3 = "",
                        name4 = "",
                        street = "Beiertheimer Allee",
                        houseNumber = "18",
                        postalCode = "76137",
                        city = "Karlsruhe",
                        countryCode = "DE",
                        emailAddress = "mail@diva-e.com",
                        telephoneNumber = "0721/9206090",
                    },
                },
                items = new List<DataContrats.In.Item> {
                    new DataContrats.In.Item {
                        materialNumber = "1110001948",
                        baseQuantity = 100,
                        baseQuantityUnit = "PCS",
                        completeDelivery = false,
                    },
                },
            }, new JsonSerializerOptions {
                WriteIndented = true,
            });
#endif
            propertyGrid2.SelectedObject = new DocumentSearchInput();
            propertyGrid4.SelectedObject = new DocumentDetailInput();
            propertyGrid6.SelectedObject = new a();
            propertyGrid8.SelectedObject = new OrderBulkDetailInput();
        }

        public class a {
            public string DocEntry { get; set; } = "";
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            ((MainConfiguration)propertyGrid1.SelectedObject).Save();
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            propertyGrid1.SelectedObject = MainConfiguration.LoadStatic();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if(MessageBox.Show(this, "Sauvegarder ?", "Sauvegarder ?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                ((MainConfiguration)propertyGrid1.SelectedObject).Save();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            var config = ((MainConfiguration)propertyGrid1.SelectedObject);
            DIAPI.SetMainConfiguration(config);
            try {
                var compdi = DIAPI.GetDIAPI();

                var errors = new List<string>();
                {
                    var ret = compdi.QueryFirstValue(@"select ""ItemCode"" from OITM where ""ItemCode"" = '{{itemcode}}'".Replace("{{itemcode}}", config.ExpenseItemCode));
                    if(ret == null || string.IsNullOrWhiteSpace(ret.ToString()))
                        errors.Add("Error with ExpenseItemCode: " + config.ExpenseItemCode);
                }

                DIAPI.DisconnectAndClean();
                if(errors.Count > 0) {
                    MessageBox.Show(this, string.Join("\r\n", errors));
                } else {
                    MessageBox.Show(this, "OK");
                }
            } catch(Exception ex) {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
        }

        private async void toolStripButton3_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                TextArea6.Text = "";
                TextArea5.Text = "";
                TextArea4.Text = "";
                TextArea3.Text = "";
                TextArea2.Text = "";

                DataContrats.In.Order order = JsonSerializer.Deserialize<DataContrats.In.Order>(TextArea1.Text);
                var r = await Task.Run(() => order.Simulation(DIAPI.GetDIAPI(), (MainConfiguration)propertyGrid1.SelectedObject));

                TextArea3.Text = JsonSerializer.Serialize(r, new JsonSerializerOptions {
                    WriteIndented = true,
                });

                TextArea4.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                tabControl2.SelectedTab = tabPage6;
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton4_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                //target.Layout = "${message}";
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                DataContrats.In.Order order = JsonSerializer.Deserialize<DataContrats.In.Order>(TextArea1.Text);

                TextArea6.Text = "";
                TextArea5.Text = "";
                TextArea4.Text = "";
                TextArea3.Text = "";
                TextArea2.Text = "";

                var r = await Task.Run(() => {
                    return order.Order(DIAPI.GetDIAPI(), (MainConfiguration)propertyGrid1.SelectedObject, (str) => {
                        TextArea2.Invoke((Action)delegate { TextArea2.Text = str; });
                        tabControl2.Invoke((Action)delegate { tabControl2.SelectedTab = tabPage6; });
                    }, (str) => {
                        TextArea5.Invoke((Action)delegate { TextArea5.Text = str; });
                        tabControl2.Invoke((Action)delegate { tabControl2.SelectedTab = tabPage8; });
                    }, (str) => {
                        TextArea6.Invoke((Action)delegate { TextArea6.Text = str; });
                        tabControl2.Invoke((Action)delegate { tabControl2.SelectedTab = tabPage9; });
                    }, true);
                });

                TextArea3.Text = JsonSerializer.Serialize(r, new JsonSerializerOptions {
                    WriteIndented = true,
                });

                TextArea4.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton5_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                //target.Layout = "${message}";
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                DataContrats.In.Order order = JsonSerializer.Deserialize<DataContrats.In.Order>(TextArea1.Text);

                TextArea6.Text = "";
                TextArea5.Text = "";
                TextArea4.Text = "";
                TextArea3.Text = "";
                TextArea2.Text = "";

                var r = await Task.Run(() => {
                    return order.Order(DIAPI.GetDIAPI(), (MainConfiguration)propertyGrid1.SelectedObject, (str) => {
                        TextArea2.Invoke((Action)delegate { TextArea2.Text = str; });
                        tabControl2.Invoke((Action)delegate { tabControl2.SelectedTab = tabPage6; });
                    }, (str) => {
                        TextArea5.Invoke((Action)delegate { TextArea5.Text = str; });
                        tabControl2.Invoke((Action)delegate { tabControl2.SelectedTab = tabPage8; });
                    }, (str) => {
                        TextArea6.Invoke((Action)delegate { TextArea6.Text = str; });
                        tabControl2.Invoke((Action)delegate { tabControl2.SelectedTab = tabPage9; });
                    }, false);
                });

                TextArea3.Text = JsonSerializer.Serialize(r, new JsonSerializerOptions {
                    WriteIndented = true,
                });

                TextArea4.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        string CSVType(Type t, string pathName = "", string propertyName = "", bool islist = false) {
            var q = "";
            foreach(var propertyInfo in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                var size = (propertyInfo.GetCustomAttribute(typeof(StringLengthAttribute)) as StringLengthAttribute)?.MaximumLength.ToString();
                var description = (propertyInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description;
                if(propertyInfo.PropertyType == typeof(string) && string.IsNullOrWhiteSpace(size)) {
                    size = "As needed";
                    if(string.IsNullOrEmpty(description)) {
                        description = "Currently unused";
                    }
                }
                var nname = (string.IsNullOrWhiteSpace(pathName) ? "" : pathName + ".") + (string.IsNullOrWhiteSpace(propertyName) ?/* t.Name*/"" : propertyName) + (islist ? "[x]" : "");
                nname = nname.Trim();

                if(propertyInfo.PropertyType.IsGenericType && (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) {
                    q += CSVType(propertyInfo.PropertyType.GetGenericArguments()[0], nname, propertyInfo.Name, true);
                } else if(propertyInfo.PropertyType.IsClass && !propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(string)) {
                    q += CSVType(propertyInfo.PropertyType, nname, propertyInfo.Name);
                } else {
                    q += (string.IsNullOrWhiteSpace(nname) ? "" : nname + ".") + propertyInfo.Name + ";" + propertyInfo.PropertyType.Name + ";" + size + ";" + description + "\r\n";
                }
            }
            return q;
        }
        private void toolStripButton6_Click(object sender, EventArgs e) {
            var basePath = @"F:\Visual Studio\stauff-interface-webshop-spryker\Documentation";
            if(!Directory.Exists(basePath))
                basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentation");

            Directory.CreateDirectory(basePath);

            const string header = "Path;Type;MaxSize;Notes\r\n";

            foreach(var t in new Type[]{
                typeof(DataContrats.In.Order),
                typeof(DataContrats.Out.OrderReturn),
                typeof(DataContrats.Out.SimulationReturn),
                typeof(DataContrats.Out.Customers),
                typeof(DataContrats.Out.ItemsPriceList),
            }) {
                File.WriteAllText(Path.Combine(basePath, t.Name + ".csv"), header + CSVType(t));
            }

            File.WriteAllText(Path.Combine(basePath, (typeof(DataContrats.In.Order)).Name + ".json"),
                JsonSerializer.Serialize(new DataContrats.In.Order {
                    debitorNumber = "",
                    vatNumber = "DE305192230",
                    customerOrderNumber = "Project #51",
                    paymentReference = "ZDKDSNQTZ2RZNN82",
                    sprykerOrderNumber = "E-Shop-FR--0815",
                    orderComments = "please deliver on tuesday",
                    completeDelivery = false,
                    freight = new DataContrats.In.Freight {
                        mode = "Express",
                    },
                    customerAddress = new DataContrats.In.Address {
                        name = "diva-e",
                        name2 = "",
                        name3 = "",
                        name4 = "",
                        street = "Beiertheimer Allee",
                        houseNumber = "18",
                        postalCode = "76137",
                        city = "Karlsruhe",
                        countryCode = "DE",
                        emailAddress = "mail@diva-e.com",
                        telephoneNumber = "0721/9206090",
                    },
                    shipping = new DataContrats.In.Shipping {
                        partnerNumber = "",
                        address = new DataContrats.In.Address {
                            name = "diva-e",
                            name2 = "",
                            name3 = "",
                            name4 = "",
                            street = "Beiertheimer Allee",
                            houseNumber = "18",
                            postalCode = "76137",
                            city = "Karlsruhe",
                            countryCode = "DE",
                            emailAddress = "mail@diva-e.com",
                            telephoneNumber = "0721/9206090",
                        },
                    },
                    items = new List<DataContrats.In.Item> {
                        new DataContrats.In.Item {
                            materialNumber = "1110001948",
                            baseQuantity = 100,
                            baseQuantityUnit = "PCS",
                            completeDelivery = false,
                        },
                    },
                }, new JsonSerializerOptions {
                    WriteIndented = true,
                }));
            File.WriteAllText(Path.Combine(basePath, (typeof(DataContrats.Out.OrderReturn)).Name + ".json"),
               JsonSerializer.Serialize(new DataContrats.Out.OrderReturn {
                   orderNumber = "123456",
                   error = "Unknown error occurred",
               }, new JsonSerializerOptions {
                   WriteIndented = true,
               }));
            File.WriteAllText(Path.Combine(basePath, (typeof(DataContrats.Out.SimulationReturn)).Name + ".json"),
               JsonSerializer.Serialize(new DataContrats.Out.SimulationReturn {
                   freight = new DataContrats.Out.Freight {
                       mode = "Standard",
                       price = new DataContrats.Common.Price {
                           baseValue = 0.10m,
                           currencyCode = "EUR"
                       },
                       taxes = new List<DataContrats.Common.Tax> {
                            new DataContrats.Common.Tax{
                                name = "VAT",
                                rate = 19.6m
                            }
                       }
                   },
                   items = new List<DataContrats.Out.Item> {
                        new DataContrats.Out.Item{
                             positionPrice = new DataContrats.Common.Price {
                                 baseValue = 228m,
                                 currencyCode = "EUR",
                             },
                             taxes = new List<DataContrats.Common.Tax>{
                                 new DataContrats.Common.Tax {
                                     name = "VAT",
                                     rate = 19.6m
                                 }
                             },
                             basePrice = new DataContrats.Common.ExtendedPrice{
                                 currencyCode = "EUR",
                                 grossValue = 2.28m,
                                 discount = 0,
                                 netValue = 2.28m
                             },
                             baseQuantity = 100,
                             baseQuantityUnit = "PCS",
                             confirmedQuantities = new List<DataContrats.Out.ConfirmedQuantity>{
                                 new DataContrats.Out.ConfirmedQuantity{
                                     quantity = 45,
                                     deliveryDate = "2022-03-16"
                                 },
                                 new DataContrats.Out.ConfirmedQuantity{
                                     quantity = 55,
                                     deliveryDate = "2022-03-16"
                                 }
                             },
                             error = null
                        }
                   },
                   error = null
               }, new JsonSerializerOptions {
                   WriteIndented = true,
               }));
            File.WriteAllText(Path.Combine(basePath, (typeof(DataContrats.Out.Customers)).Name + ".json"),
               JsonSerializer.Serialize(new DataContrats.Out.Customers {
                   Debitors = new List<DataContrats.Out.Customer> {
                       new DataContrats.Out.Customer {
                            DebitorNumber = "C150040",
                            DebitorName = "ALDIANCE",
                            PriceList = 2,
                            Contacts = new List<DataContrats.Out.CustomerContact> {
                                new DataContrats.Out.CustomerContact {
                                    ContactID= "8211",
                                    FirstName= null,
                                    LastName= "Gaetane BELIN",
                                    EMailAddress= "gaetane.belin@sonepar.fr",
                                    Salutation= "Mme",
                                    Gender= "female",
                                    Language= "FR",
                                }
                            },
                            Addresses = new List<DataContrats.Out.CustomerAddress> {
                                new DataContrats.Out.CustomerAddress {
                                    AddressID= "ALDIANCE (42).3",
                                    Name1= "ALDIANCE (42)",
                                    Name2= "",
                                    Name3= null,
                                    Name4= null,
                                    Street= "5, rue Victor Grignard",
                                    ZipCode= "42000",
                                    City= "Saint-Etienne",
                                    CountryCode= "FR",
                                    IsBilAdr= false,
                                    IsDefShip= false,
                                },
                                new DataContrats.Out.CustomerAddress {
                                    AddressID= "ALDIANCE (73).2",
                                    Name1= "ALDIANCE (73)",
                                    Name2= "",
                                    Name3= null,
                                    Name4= null,
                                    Street= "401 RUE DES CHAMPAGNES",
                                    ZipCode= "73290",
                                    City= "LA MOTTE SERVOLEX",
                                    CountryCode= "FR",
                                    IsBilAdr= false,
                                    IsDefShip= false,
                                },
                                new DataContrats.Out.CustomerAddress {
                                    AddressID= "ALDIANCE (CLUSES).1",
                                    Name1= "ALDIANCE (CLUSES)",
                                    Name2= "34, rue de la sapini\u00E9re",
                                    Name3= null,
                                    Name4= null,
                                    Street= "ZI DE GLAIZY",
                                    ZipCode= "74300",
                                    City= "THYEZ",
                                    CountryCode= "FR",
                                    IsBilAdr= false,
                                    IsDefShip= true,
                                },
                                new DataContrats.Out.CustomerAddress {
                                    AddressID= "ALDIANCE Fact.1",
                                    Name1= "ALDIANCE Fact",
                                    Name2= "",
                                    Name3= null,
                                    Name4= null,
                                    Street= "ZI DE GLAIZY - THYEZ",
                                    ZipCode= "74304",
                                    City= "THYEZ CEDEX",
                                    CountryCode= "FR",
                                    IsBilAdr= true,
                                    IsDefShip= false,
                                }
                            },
                       }
                   }
               }, new JsonSerializerOptions {
                   WriteIndented = true,
               }));
            File.WriteAllText(Path.Combine(basePath, (typeof(DataContrats.Out.ItemsPriceList)).Name + ".json"),
               JsonSerializer.Serialize(new DataContrats.Out.ItemsPriceList {
                   Items = new List<DataContrats.Out.ItemPriceList> {
                       new DataContrats.Out.ItemPriceList {
                            ItemCode = "6100043000",
                             PriceList = 2,
                             Currency = "EUR",
                             Qty = 1,
                             Price = 42.72m,
                             //Unit = "",
                       }
                   },
               }, new JsonSerializerOptions {
                   WriteIndented = true,
               }));
        }

        private void Form1_ResizeEnd(object sender, EventArgs e) {
            toolStripProgressBar1.Width = this.Width / 5 * 4;
        }

        private void button2_Click(object sender, EventArgs e) {
            var config = ((MainConfiguration)propertyGrid1.SelectedObject);
            DIAPI.SetMainConfiguration(config);
            try {
                var compdi = DIAPI.GetDIAPI();

                compdi.CheckAndCreateUserField("ORDR", "FRANCO", "Franco", 10);
                compdi.CheckAndCreateUserField("ORDR", "NCdeWeb", "N° Cde Web", 50);
                compdi.CheckAndCreateUserField("ORDR", "TelWeb", "TelWeb", 20);
                compdi.CheckAndCreateUserField("ORDR", "EmailWeb", "EmailWeb", 100);
                compdi.CheckAndCreateUserField("ORDR", "NOMCORRE", "Nom correspondant", 100);
                compdi.CheckAndCreateUserField("ORDR", "AGREM", "Agrément", 100, SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Price);
                compdi.CheckAndCreateUserField("ORDR", "ENTJUR", "Entité Juridique", 9);
                compdi.CheckAndCreateUserField("ORDR", "MINI", "Mini Cde", 10, SAPbobsCOM.BoFieldTypes.db_Numeric);
                compdi.CheckAndCreateUserField("ORDR", "GROUPAGE", "Groupage", 20);
                compdi.CheckAndCreateUserField("ORDR", "REMC", "Remarques", 20);
                compdi.CheckAndCreateUserField("ORDR", "ACCENC", "Autoriser dépasst agrément ?", 10, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None,
                    new Dictionary<string, string> {
                        { "Oui", "Y" },
                        { "Non", "N" },
                    }, "N");
                compdi.CheckAndCreateUserField("ORDR", "AppPL", "ApprouvéPickingList", 10, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None,
                    new Dictionary<string, string> {
                        { "Oui", "Y" },
                        { "Non", "N" },
                        { "DirectUsine", "D" },
                        { "Attente Accord Client", "A" },
                        { "Attente Accord Compta.", "C" },
                        { "Prévisions", "P" },
                        { "Ferme en Attente", "F" },
                        { "Société Fermée", "S" },
                    }, "N");
                compdi.CheckAndCreateUserField("RDR1", "FORCE", "Forcer la préparation", 10, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None,
                    new Dictionary<string, string> {
                        { "Oui", "O" },
                        { "Non", "N" },
                    }, "N");
                compdi.CheckAndCreateUserField("RDR1", "STPREPA", "Statut préparation", 10, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoFldSubTypes.st_None,
                    new Dictionary<string, string> {
                        { "En préparation", "0" },
                        { "Non Dispo", "1" },
                        { "A préparer", "2" },
                        { "Livré", "3" },
                    });
                compdi.CheckAndCreateUserField("RDR1", "CodeSTD", "Code STD", 11);
                compdi.CheckAndCreateUserField("RDR1", "NumCat", "U_NumCat", 11);

                compdi.CheckAndCreateUserField("OCRD", "WEBREMISE", "Remise Webshop", 254, SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Percentage);
                compdi.CheckAndCreateUserField("OITB", "WEBREMISE", "Remise Webshop", 254, SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoFldSubTypes.st_Percentage);
                compdi.CheckAndCreateUserField("OITB", "WEBREMISEFROM", "Remise Webshop de", 254, SAPbobsCOM.BoFieldTypes.db_Date);
                compdi.CheckAndCreateUserField("OITB", "WEBREMISETO", "Remise Webshop à", 254, SAPbobsCOM.BoFieldTypes.db_Date);

                compdi.CheckAndCreateUserField("OCRD", "WEBCONDDUMMY", "Conditionnement Vente Dummy", 11,
                        SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None);
                compdi.CheckAndCreateUserField("OITM", "WEBCONDDUMMY", "Conditionnement Vente Dummy", 11,
                        SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoFldSubTypes.st_None);
                DIAPI.DisconnectAndClean();
                MessageBox.Show(this, "OK");
            } catch(Exception ex) {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private async void toolStripButton7_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                //target.Layout = "${message}";
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                DataContrats.In.Order order = JsonSerializer.Deserialize<DataContrats.In.Order>(TextArea1.Text);

                TextArea6.Text = "";
                TextArea5.Text = "";
                TextArea4.Text = "";
                TextArea3.Text = "";
                TextArea2.Text = "";

                var r = await Task.Run(() => Actions.Customers(DIAPI.GetDIAPI(), (MainConfiguration)propertyGrid1.SelectedObject));

                TextArea3.Text = JsonSerializer.Serialize(r, new JsonSerializerOptions {
                    WriteIndented = true,
                });
                tabControl2.SelectedTab = tabPage6;

                TextArea4.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton8_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                //target.Layout = "${message}";
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                DataContrats.In.Order order = JsonSerializer.Deserialize<DataContrats.In.Order>(TextArea1.Text);

                TextArea6.Text = "";
                TextArea5.Text = "";
                TextArea4.Text = "";
                TextArea3.Text = "";
                TextArea2.Text = "";

                var r = await Task.Run(() => Actions.PriceLists(DIAPI.GetDIAPI(), (MainConfiguration)propertyGrid1.SelectedObject, 2));

                TextArea3.Text = JsonSerializer.Serialize(r, new JsonSerializerOptions {
                    WriteIndented = true,
                });
                tabControl2.SelectedTab = tabPage6;

                TextArea4.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton9_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                if(((DocumentSearchInput)propertyGrid2.SelectedObject).DocType == DocumentSearch.DocType.Unknown)
                    ((DocumentSearchInput)propertyGrid2.SelectedObject).DocType = DocumentSearch.DocType.Order;

                var r = await Task.Run(() => DocumentSearch.Search(DIAPI.GetDIAPI(), (DocumentSearchInput)propertyGrid2.SelectedObject));
                propertyGrid3.SelectedObject = r;
                tabControl3.SelectedTab = tabPage12;

                textBox1.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton10_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                if(((DocumentSearchInput)propertyGrid2.SelectedObject).DocType == DocumentSearch.DocType.Unknown)
                    ((DocumentSearchInput)propertyGrid2.SelectedObject).DocType = DocumentSearch.DocType.Delivery;

                var r = await Task.Run(() => DocumentSearch.Search(DIAPI.GetDIAPI(), (DocumentSearchInput)propertyGrid2.SelectedObject));
                propertyGrid3.SelectedObject = r;
                tabControl3.SelectedTab = tabPage12;

                textBox1.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton11_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                if(((DocumentSearchInput)propertyGrid2.SelectedObject).DocType == DocumentSearch.DocType.Unknown)
                    ((DocumentSearchInput)propertyGrid2.SelectedObject).DocType = DocumentSearch.DocType.Invoice;

                var r = await Task.Run(() => DocumentSearch.Search(DIAPI.GetDIAPI(), (DocumentSearchInput)propertyGrid2.SelectedObject));
                propertyGrid3.SelectedObject = r;
                tabControl3.SelectedTab = tabPage12;

                textBox1.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton12_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                var r = await Task.Run(() => DocumentSearch.Detail(DIAPI.GetDIAPI(), DocumentSearch.DocType.Order, (DocumentDetailInput)propertyGrid4.SelectedObject));
                propertyGrid5.SelectedObject = r;
                tabControl4.SelectedTab = tabPage16;

                textBox2.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton13_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                var r = await Task.Run(() => DocumentSearch.Detail(DIAPI.GetDIAPI(), DocumentSearch.DocType.Delivery, (DocumentDetailInput)propertyGrid4.SelectedObject));
                propertyGrid5.SelectedObject = r;
                tabControl4.SelectedTab = tabPage16;

                textBox2.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton14_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                var r = await Task.Run(() => DocumentSearch.Detail(DIAPI.GetDIAPI(), DocumentSearch.DocType.Invoice, (DocumentDetailInput)propertyGrid4.SelectedObject));
                propertyGrid5.SelectedObject = r;
                tabControl4.SelectedTab = tabPage16;

                textBox2.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton15_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                try {
                    var r = await DocumentSearch.Pdf(
                        DIAPI.GetDIAPI(),
                        DocumentSearch.DocType.Order,
                        ((a)propertyGrid6.SelectedObject).DocEntry,
                        ((MainConfiguration)propertyGrid1.SelectedObject).CoresuiteOrderPdf);
                    propertyGrid7.SelectedObject = new { Result = r };
                } catch(Exception ex) {
                    MessageBox.Show(this, ex.ToString());
                    propertyGrid7.SelectedObject = "";
                }
                tabControl5.SelectedTab = tabPage20;

                textBox3.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton16_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                try {
                    var r = await DocumentSearch.Pdf(
                        DIAPI.GetDIAPI(),
                        DocumentSearch.DocType.Delivery,
                        ((a)propertyGrid6.SelectedObject).DocEntry,
                        ((MainConfiguration)propertyGrid1.SelectedObject).CoresuiteDeliveryPdf);
                    propertyGrid7.SelectedObject = new { Result = r };
                } catch(Exception ex) {
                    MessageBox.Show(this, ex.ToString());
                    propertyGrid7.SelectedObject = "";
                }
                tabControl5.SelectedTab = tabPage20;

                textBox3.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton17_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                try {
                    var r = await DocumentSearch.Pdf(
                        DIAPI.GetDIAPI(),
                        DocumentSearch.DocType.Invoice,
                        ((a)propertyGrid6.SelectedObject).DocEntry,
                        ((MainConfiguration)propertyGrid1.SelectedObject).CoresuiteInvoicePdf);

                    if(string.IsNullOrEmpty(r)) {
                        r = await DocumentSearch.Pdf(
                        DIAPI.GetDIAPI(),
                        DocumentSearch.DocType.CreditNote,
                        ((a)propertyGrid6.SelectedObject).DocEntry,
                        ((MainConfiguration)propertyGrid1.SelectedObject).CoresuiteCreditNotePdf);
                    }
                    propertyGrid7.SelectedObject = new { Result = r };
                } catch(Exception ex) {
                    MessageBox.Show(this, ex.ToString());
                    propertyGrid7.SelectedObject = "";
                }
                tabControl5.SelectedTab = tabPage20;

                textBox3.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton18_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                try {
                    var r = await Task.Run(() => DocumentSearch.OrderBulkDetail(DIAPI.GetDIAPI(), (OrderBulkDetailInput)propertyGrid8.SelectedObject));
                    propertyGrid9.SelectedObject = r;
                } catch(Exception ex) {
                    MessageBox.Show(this, ex.ToString());
                    propertyGrid9.SelectedObject = "";
                }
                tabControlOrderBulkDetail.SelectedTab = tabPage24;

                textBox4.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStripButton19_Click(object sender, EventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                //if(((DocumentSearchInput)propertyGrid2.SelectedObject).DocType == DocumentSearch.DocType.Unknown)
                //((DocumentSearchInput)propertyGrid2.SelectedObject).DocType = DocumentSearch.DocType.Order;

                var r = await Task.Run(() => DocumentSearch.Search(DIAPI.GetDIAPI(), (DocumentSearchInput)propertyGrid2.SelectedObject));
                propertyGrid3.SelectedObject = r;
                tabControl3.SelectedTab = tabPage12;

                textBox1.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }

        private async void toolStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            if(!toolStripProgressBar1.Visible) {
                toolStripProgressBar1.Visible = true;
                MemoryTarget target = new MemoryTarget("test");
                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, target);
                NLog.LogManager.Configuration = config;

                var r = await Task.Run(() => DocumentSearch.Search(DIAPI.GetDIAPI(), (DocumentSearchInput)propertyGrid2.SelectedObject));
                propertyGrid3.SelectedObject = r;
                tabControl3.SelectedTab = tabPage12;

                textBox1.Text = string.Join("\r\n", target.Logs);
                NLog.LogManager.Configuration.RemoveRuleByName("test");
                toolStripProgressBar1.Visible = false;
            }
        }
    }
}
