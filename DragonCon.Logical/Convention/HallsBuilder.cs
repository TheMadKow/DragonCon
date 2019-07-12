using System;
using System.Linq;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;

namespace DragonCon.Logical.Convention
{
    public class HallsBuilder : BuilderBase<Hall>
    {
        public HallsBuilder(ConventionBuilder builder, ConventionWrapper convention) : 
            base(builder, convention) { }

        public ConventionBuilder AddHall(string hallName, string hallDesc, 
            int firstTable, int lastTable)
        {
            ThrowIfHallNameExists(hallName, string.Empty);
            ThrowIfTablesInvalid(firstTable, lastTable);
            var testHall = new Hall
            {
                Description = hallDesc,
                Name = hallName,
                FirstTable = firstTable,
                LastTable = lastTable
                
            };
            ThrowIfTablesExists(testHall, string.Empty);
            Convention.Halls.Add(testHall);
            return Parent;
        }

        private void ThrowIfTablesExists(Hall testHall, string hallId)
        {
            foreach (var hall in Convention.Halls)
            {
                if (hall.Id == hallId)
                    continue;
                
                foreach (var table in testHall.Tables)
                {
                    if (hall.Tables.Contains(table))
                        throw new Exception("Invalid Table Range.");
                }
            }
        }

        private void ThrowIfTablesInvalid(int firstTable, int lastTable)
        {
            if (firstTable <= 0 || lastTable <= 0) 
                throw new Exception("Hall Numbers Must Be Positive");
            if (lastTable < firstTable)
                throw new Exception("Last Table must be greater than First Table");
        }

        private void ThrowIfHallNameExists(string hallName, string hallId)
        {
            if (Convention.Halls.Any(x => x.Name == hallName && x.Id != hallId))
                throw new Exception("Hall Name Already Exists");
        }

        private void ThrowIfHallDoesntExists(string hallKey)
        {
            if (KeyExists(hallKey) == false)
                throw new Exception("Hall doesn't Exists");

        }

        public ConventionBuilder RemoveHall(string hallId)
        {
            ThrowIfHallDoesntExists(hallId);
            var removedHall = this[hallId];
            Convention.Halls.Remove(removedHall);
            Parent.DeletedEntityIds.Add(hallId);
            return Parent;
        }

        public ConventionBuilder UpdateHall(string hallId, 
            string name, string desc, int firstTable, int lastTable)
        {
            ThrowIfHallNameExists(name, hallId);
            ThrowIfTablesInvalid(firstTable, lastTable);
            var testHall = new Hall
            {
                FirstTable = firstTable,
                LastTable = lastTable
            };
            ThrowIfTablesExists(testHall, hallId);
            var updated = this[hallId];
            updated.Name = name;
            updated.Description = desc;
            updated.FirstTable = firstTable;
            updated.LastTable = lastTable;
            return Parent;


        }

    }
}