using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using stauff_interface_webshop_spryker_ui.DataContrats.In;
using stauff_interface_webshop_spryker_ui.Extensions;

namespace UnitTests {
    [TestClass]
    public class SimulationTests {
        [TestMethod]
        public void Cas_2023_08_18() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 63,
                    Mag1 = 10241,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 08, 14),
                        Quantite = 75,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 08, 29),
                        Quantite = 1600,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 09, 05),
                        Quantite = 7575,
                        Type = "A",
                    },
                },
                25, // Quantitée voulue
                false, // Est client dummy
                new DateTime(2023, 08, 18)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(25, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2023-08-19", r[0].deliveryDate, "Date Line 1");
        }

        [TestMethod]
        public void Exemple_20221128_6020000507_Q1000() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 63,
                    Mag1 = 0,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 13),
                        Quantite = 278,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 10),
                        Quantite = 300,
                        Type = "A",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 10),
                        Quantite = 300,
                        Type = "A",
                    },
                },
                1000, // Quantitée voulue
                false, // Est client dummy
                new DateTime(2022, 11, 28)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(2, r.Count, "Count");
            //
            Assert.AreEqual(322, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2023-01-12", r[0].deliveryDate, "Date Line 1");
            Assert.AreEqual(678, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2023-02-06", r[1].deliveryDate, "Date Line 2");
        }
        [TestMethod]
        public void Exemple_20221123_6020000507_Q1000() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 63,
                    Mag1 = 127,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 25),
                        Quantite = 127,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 13),
                        Quantite = 273,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 10),
                        Quantite = 300,
                        Type = "A",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 10),
                        Quantite = 300,
                        Type = "A",
                    },
                },
                1000, // Quantitée voulue
                false, // Est client dummy
                new DateTime(2022, 11, 23)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(2, r.Count, "Count");
            //
            Assert.AreEqual(327, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2023-01-12", r[0].deliveryDate, "Date Line 1");
            Assert.AreEqual(673, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2023-02-01", r[1].deliveryDate, "Date Line 2");
        }
        [TestMethod]
        public void Exemple_20221027_1130005333_Q4001() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2425,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 27),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 03),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 15),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                4001, // Quantitée voulue
                false, // Est client dummy
                new DateTime(2022, 10, 27)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(3, r.Count, "Count");
            //
            Assert.AreEqual(1300, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-28", r[0].deliveryDate, "Date Line 1");
            Assert.AreEqual(1700, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2022-11-10", r[1].deliveryDate, "Date Line 2");
            Assert.AreEqual(1025, r[2].quantity, "Quantity Line 3");
            Assert.AreEqual("2022-11-17", r[2].deliveryDate, "Date Line 3");
        }
        [TestMethod]
        public void Exemple_20221027_1130005368_Q801() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 418,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 27),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 28),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 200,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 500,
                        Type = "A",
                    },
                },
                801, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 27)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(3, r.Count, "Count");
            //
            Assert.AreEqual(275, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-28", r[0].deliveryDate, "Date Line 1");
            Assert.AreEqual(300, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2022-11-02", r[1].deliveryDate, "Date Line 2");
            Assert.AreEqual(250, r[2].quantity, "Quantity Line 3");
            Assert.AreEqual("2022-11-17", r[2].deliveryDate, "Date Line 3");
        }
        [TestMethod]
        public void Exemple_20221026_1130005333_Q4001() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2500,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 27),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 15),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                4001, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 26)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(3, r.Count, "Count");
            //
            Assert.AreEqual(1300, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-27", r[0].deliveryDate, "Date Line 1");
            Assert.AreEqual(1700, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2022-11-10", r[1].deliveryDate, "Date Line 2");
            Assert.AreEqual(1025, r[2].quantity, "Quantity Line 3");
            Assert.AreEqual("2022-11-16", r[2].deliveryDate, "Date Line 3");
        }
        [TestMethod]
        public void Exemple_20221026_1130005333_Q1380() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2500,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 27),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 15),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                1380, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 26)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(2, r.Count, "Count");
            //
            Assert.AreEqual(1300, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-27", r[0].deliveryDate, "Date Line 1");
            Assert.AreEqual(100, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2022-11-10", r[1].deliveryDate, "Date Line 2");
        }
        [TestMethod]
        public void Exemple_20221026_1130005333_Q1300() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2500,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 27),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 15),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                1300, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 26)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(1300, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-27", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130004021_Q1() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 4719,
                    Mag2 = 0,
                    SalFactor2 = 50,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 20),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 26),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 04),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 3375,
                        Type = "A",
                    },
                },
                1, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130004021_Q24() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 4719,
                    Mag2 = 0,
                    SalFactor2 = 50,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 20),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 26),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 04),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 3375,
                        Type = "A",
                    },
                },
                1, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130004021_Q25() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 4719,
                    Mag2 = 0,
                    SalFactor2 = 50,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 20),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 26),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 04),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 3375,
                        Type = "A",
                    },
                },
                1, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130004021_Q30() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 4719,
                    Mag2 = 0,
                    SalFactor2 = 50,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 20),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 26),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 04),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 3375,
                        Type = "A",
                    },
                },
                1, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130004021_Q50() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 4719,
                    Mag2 = 0,
                    SalFactor2 = 50,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 20),
                        Quantite = 300,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 26),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 04),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 3375,
                        Type = "A",
                    },
                },
                1, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130005333_Q1() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2525,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 25),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                1, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(25, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130005333_Q24() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2525,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 25),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                24, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(25, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130005333_Q25() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2525,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 25),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                25, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(25, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221108_1130005333_Q30_ClientDummy() {
            //pas un exemple donné par le client
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2525,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 25),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                30, // Quantitée voulue 
                true, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(30, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130005333_Q30() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2525,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 25),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                30, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221019_1130005333_Q50() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 14,
                    Mag1 = 2525,
                    Mag2 = 0,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 25),
                        Quantite = 25,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 31),
                        Quantite = 1000,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 02),
                        Quantite = 100,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 11, 08),
                        Quantite = 2000,
                        Type = "A",
                    },
                },
                50, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 19)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(50, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-20", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20221006_61000949861_Q10() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line { // résultat vue sql STAUFF_webshop_dispo_stock simulé
                    LeadTime = 210,
                    Mag1 = 924,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 7
                },
                new List<Item.Stauff_webshop_dispo_date_line> { // résultat vue sql STAUFF_webshop_dispo_date simulé
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2023, 01, 09),
                        Quantite = 1452,
                        Type = "A",
                    },
                },
                10, // Quantitée voulue 
                false, // Est client dummy
                new DateTime(2022, 10, 06)); // Date référence à considérer comme aujourd'hui

            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(10, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-07", r[0].deliveryDate, "Date Line 1");
        }

        // ne trouve pas de mail client confirmant ce test comme valide
        /*[TestMethod]
        public void Exemple_20221021_1130005333_Q1() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 15,
                    Mag1 = 2197,
                    Mag2 = 2175,
                    SalFactor2 = 25,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> {
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 09),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 10),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 12),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 16),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 23),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 31),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 06, 16),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 06, 30),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 07, 03),
                        Quantite = 20,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 07, 04),
                        Quantite = 20,
                        Type = "V",
                    },
                },
                1,
                new DateTime(2022, 09, 29));
            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(25, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-10-21", r[0].deliveryDate, "Date Line 1");
        }*/
        [TestMethod]
        public void Exemple_20220922_1020016160_Q1() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 53,
                    Mag1 = 25,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> {
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 26),
                        Quantite = 20,
                        Type = "V",
                    },
                },
                1,
                false, // Est client dummy
                new DateTime(2022, 09, 21));
            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(1, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-09-23", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20220922_1020000328_Q1() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 15,
                    Mag1 = 5,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                null,
                1,
                false, // Est client dummy
                new DateTime(2022, 09, 21));
            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(1, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-09-23", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_20220922_6100043000_Q1() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 102,
                    Mag1 = 132,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> {
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 26),
                        Quantite = 1,
                        Type = "V",
                    },
                },
                1,
                false, // Est client dummy
                new DateTime(2022, 09, 21));
            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(1, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-09-23", r[0].deliveryDate, "Date Line 1");
        }
        //pas sûr que ces exemples soient fournis par le client donc aucune idée si valide
        /*[TestMethod]
        public void Exemple_6100071978() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 21,
                    Mag1 = 1,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 9
                },
                new List<Item.Stauff_webshop_dispo_date_line> {
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 05, 09),
                        Quantite = 1,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 26),
                        Quantite = 1,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 20),
                        Quantite = 10,
                        Type = "A",
                    },
                },
                11,
                new DateTime(2022, 09, 22));
            Assert.AreEqual(2, r.Count, "Count");
            //
            Assert.AreEqual(9, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-09-29", r[0].deliveryDate, "Date Line 1");
            //
            Assert.AreEqual(2, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2022-10-20", r[1].deliveryDate, "Date Line 2");
        }
        [TestMethod]
        public void Exemple_1730000015() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 39 - 7,
                    Mag1 = 163,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 2
                },
                new List<Item.Stauff_webshop_dispo_date_line> {
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 07, 01),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 07, 21),
                        Quantite = 2,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 08, 26),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 23),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 24),
                        Quantite = 1,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 10, 21),
                        Quantite = 50,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 20),
                        Quantite = 225,
                        Type = "A",
                    },
                },
                100,
                new DateTime(2022, 09, 22));
            Assert.AreEqual(1, r.Count, "Count");
            //
            Assert.AreEqual(100, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-09-22", r[0].deliveryDate, "Date Line 1");
        }
        [TestMethod]
        public void Exemple_1730000226() {
            var item = new Item();
            var r = item.CalculateConfirmQuantities(
                new Item.Stauff_webshop_dispo_stock_line {
                    LeadTime = 28,
                    Mag1 = 8,
                    Mag2 = 0,
                    SalFactor2 = 1,
                    U_Jourrecep = 9
                },
                new List<Item.Stauff_webshop_dispo_date_line> {
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 19),
                        Quantite = 1,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 29),
                        Quantite = 5,
                        Type = "V",
                    },
                    new Item.Stauff_webshop_dispo_date_line {
                        Date = new DateTime(2022, 09, 21),
                        Quantite = 4,
                        Type = "A",
                    },
                },
                10,
                new DateTime(2022, 09, 22));
            Assert.AreEqual(3, r.Count, "Count");
            //
            Assert.AreEqual(2, r[0].quantity, "Quantity Line 1");
            Assert.AreEqual("2022-09-20", r[0].deliveryDate, "Date Line 1");
            //
            Assert.AreEqual(4, r[1].quantity, "Quantity Line 2");
            Assert.AreEqual("2022-09-30", r[1].deliveryDate, "Date Line 2");
            //
            Assert.AreEqual(4, r[2].quantity, "Quantity Line 3");
            Assert.AreEqual("2022-10-27", r[2].deliveryDate, "Date Line 3");
        }*/
    }
}
