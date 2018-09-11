using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Convention;
using DragonCon.Modeling.Models.Wrappers;

namespace DragonCon.Logic.Convention
{
    public class HallsBuilder
    {
        private readonly ConventionBuilder _builder;
        private readonly ConventionWrapper _convention;
        public HallsBuilder(ConventionBuilder builder, ConventionWrapper convention)
        {
            _convention = convention;
            _builder = builder;
        }

        public ConventionBuilder AddHall(string hallName, string hallDesc)
        {
            ThrowIfHallExists(hallName);
            _convention.NameAndHall.Add(hallName, new Hall()
            {
                Name = hallName,
                Description = hallDesc,
                Tables = new List<string>()
            });
            return _builder;
        }

        private void ThrowIfHallExists(string hallName)
        {
            if (_convention.NameAndHall.ContainsKey(hallName))
                throw new Exception("Hall Already Exists");
        }

        private void ThrowIfHallDoesntExists(string hallName)
        {
            if (!_convention.NameAndHall.ContainsKey(hallName))
                throw new Exception("Hall doesn't Exists");

        }

        public ConventionBuilder SetDescription(string hallName, string hallDesc)
        {
            ThrowIfHallDoesntExists(hallName);
            _convention.NameAndHall[hallName].Description = hallDesc;
            return _builder;
        }

        public ConventionBuilder SetHallTables(string hallName, string[] tables)
        {
            ThrowIfHallDoesntExists(hallName);
            _convention.NameAndHall[hallName].Tables = tables.ToList();
            return _builder;
        }

        public ConventionBuilder RenameHall(string hallNameOld, string hallNameNew)
        {
            ThrowIfHallDoesntExists(hallNameOld);
            ThrowIfHallExists(hallNameNew);

            var oldHall = _convention.NameAndHall[hallNameOld];
            oldHall.Name = hallNameNew;
            _convention.NameAndHall.Remove(hallNameOld);
            _convention.NameAndHall.Add(hallNameNew, oldHall);
            return _builder;
        }

        public ConventionBuilder RemoveHall(string hallName)
        {
            ThrowIfHallDoesntExists(hallName);
            _convention.NameAndHall.Remove(hallName);
            return _builder;
        }

        public static string[] RoomsFromNumericRange(int from, int to)
        {
            if (to < from)
                return new string[0];

            var size = to - from + 1;
            var results = new List<string>(size);
            for (int i = from; i <= to; i++)
            {
                results.Add(i.ToString());
            }

            return results.ToArray();
        }
    }
}