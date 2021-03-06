﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using дипломная_работа.Controls;
using дипломная_работа.Model;
using дипломная_работа.Resources;
using Word = Microsoft.Office.Interop.Word;

namespace дипломная_работа.Helpers
{
    class Importer
    {
        Word._Application application;
        Word._Document document;
        public Importer()
        {
            Tools.SetNumberDecimalSeparator();
            application = new Word.Application();
            try
            {
                document = application.Documents.Add();
            }
            catch (Exception error)
            {
                document.Close();
                application.Quit();
                document = null;
                application = null;
            }
            application.Visible = true;
        }
        public void MakeReport(Result Result, Querry Querry, List<ResultItemInfo> infos)
        {
            object start = 0;
            object end = 0;

            Word.Range rng = document.Range(ref start, ref end);
            rng.Text = "Маршрут: "+Environment.NewLine+Result.GetDescription() + Environment.NewLine;
            rng.Text += "Итоговая стоимость: " + String.Format("{0:f2} рублей",Result.GetCost()) + Environment.NewLine + Environment.NewLine;
            rng.Text += "Стоимость авиабилетов: " + String.Format("{0:f2} рублей", Result.FlightCostTotal) + Environment.NewLine + Environment.NewLine;
            rng.Text += "Стоимость отелей: " + String.Format("{0:f2} рублей", Result.HotelCost) + Environment.NewLine + Environment.NewLine;
            rng.Text += "Авиабилеты:" + Environment.NewLine;
            int i = 1;
            foreach(var item in Result.Route)
            {
                if (item.ArrivalInfo == null)
                    continue;
                var to = item.ArrivalInfo;
                rng.Text += i + ") " + (item.ArrivalInfo.Segment.Count-1).ToString() + " пересадок";
                int k = 1;
                foreach(var segment in to.Segment)
                {
                    Airport origin = CommonData.Airports.Where(a => a.Code == segment.Leg[0].Origin).FirstOrDefault(), destinamion = CommonData.Airports.Where(a => a.Code == segment.Leg[0].Destination).FirstOrDefault();
                    rng.Text += "       "+k+". " + (origin.Name_rus == "" ? origin.Name : origin.Name_rus) + " " + segment.Leg[0].OriginTerminal + "-" + (destinamion.Name_rus == "" ? destinamion.Name : destinamion.Name_rus) + " " + segment.Leg[0].DestinationTerminal + Environment.NewLine;
                    var depTime = Computer.ParseDateTime(segment.Leg[0].DepartureTime);
                    var ArTime = Computer.ParseDateTime(segment.Leg[0].ArrivalTime);
                    rng.Text += "       Дата вылета: " + depTime.ToShortDateString() + " " + depTime.ToShortTimeString() + Environment.NewLine;
                    rng.Text += "       Дата посадки: " + ArTime.ToShortDateString() + " " + ArTime.ToShortTimeString() + Environment.NewLine;
                    rng.Text += "       Класс: " + segment.Cabin + Environment.NewLine;
                    rng.Text += "       Авиакомпания: " + segment.Flight.Carrier + Environment.NewLine;
                    k++;
                }
                i++;
            }
            rng.Text += Environment.NewLine + "Отели:" + Environment.NewLine;
            
            foreach (var item in infos)
            {
                if (item.IsNativeTown)
                    continue;
                rng.Text +=item.resItem.Town.Name_rus==""? item.resItem.Town.Name: item.resItem.Town.Name_rus+":" + Environment.NewLine;
                i = 1;
                foreach(var Room in item.Rooms)
                {
                    rng.Text += "   "+i+") " + "Название: " + Room.hotel.property_name + Environment.NewLine;
                    try
                    {
                        rng.Text += "   " + "Адрес: " + Room.hotel.address.line1 + Environment.NewLine;
                    }
                    catch { }
                    rng.Text += "   " + "Описание: " + Room.description.Replace('\"', '\0').Replace('{', '\0').Replace('}', '\0') + Environment.NewLine;
                    rng.Text += "   " + "Тип комнаты: " + Room.room_type_info.room_type + ", " + Room.room_type_info.bed_type + ", " + Room.room_type_info.number_of_beds + " beds" + Environment.NewLine;
                    try
                    {
                        rng.Text += "   " + "Рейтинг: " + Room.hotel.awards.Average(a => double.Parse(a.rating)) + Environment.NewLine;
                    }
                    catch { }
                    CurrencyConverter cc = CommonData.CurrencyConverter;
                    rng.Text += "   " + "Стоимость: " + String.Format("{0:f2} rub", cc.getRub(Room.total_amount).amount) + Environment.NewLine;
                    rng.Text += "   " + "Контакты: " + Environment.NewLine;
                    foreach(var c in Room.hotel.contacts)
                        rng.Text += "       -" +c.type+ ": " +c.detail+ Environment.NewLine;
                    i++;
                }
            }
        }
    }
}
