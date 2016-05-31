using SlidingPuzzle.Code.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Code.Database
{
    public class DatabaseHandler
    {
        private DataSet data;

        public DatabaseHandler()
        {
            data = new DataSet();

            DataTable table = new DataTable("times_hiscores");
            table.Columns.Add("entry", typeof(HiScoreData));
            data.Tables.Add(table);

            table = new DataTable("moves_hiscores");
            table.Columns.Add("entry", typeof(HiScoreData));
            data.Tables.Add(table);
        }

        public void LoadData()
        {
            if (!File.Exists(@"SliderPuzzleDB.xml"))
                return;

            foreach (DataTable table in data.Tables)
                table.BeginLoadData();

            data.ReadXml(@"SliderPuzzleDB.xml");

            foreach (DataTable table in data.Tables)
                table.EndLoadData();
        }

        public void SaveData()
        {
            FileStream fs = new FileStream(@"SliderPuzzleDB.xml", FileMode.Create);
            data.WriteXml(fs);
        }

        public bool CheckTimeHiScoresAsync(int time)
        {
            foreach (HiScoreData data in GetTopFiveTimeHiScoresAsync())
            {
                if (data.Value < time)
                    return true;
            }
            return false;
        }

        public bool CheckMovesHiScoresAsync(int moves)
        {

            foreach (HiScoreData data in GetTopFiveMovesHiScoresAsync())
            {
                if (data.Value < moves)
                    return true;
            }
            return false;
        }

        public void InsertNewHiScoreAsync(string name, int time, int moves)
        {
            data.Tables["times_hiscores"].Rows.Add(new HiScoreData(name, time));
            data.Tables["moves_hiscores"].Rows.Add(new HiScoreData(name, moves));
            SaveData();
        }

        public IEnumerable<HiScoreData> GetTopFiveTimeHiScoresAsync()
        {
            var results = (from row in data.Tables["times_hiscores"].AsEnumerable()
                           orderby row.Field<HiScoreData>("entry").Value
                           select row.Field<HiScoreData>("entry")).Take(5);
            return results;
        }

        public IEnumerable<HiScoreData> GetTopFiveMovesHiScoresAsync()
        {
            var results = (from row in data.Tables["moves_hiscores"].AsEnumerable()
                           orderby row.Field<HiScoreData>("entry").Value
                           select row.Field<HiScoreData>("entry")).Take(5);
            return results;
        }
    }
}
