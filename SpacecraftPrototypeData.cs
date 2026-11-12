using System;
using System.Linq;
using Mafi;
using Mafi.Core.Entities.Static;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;
using Mafi.Core.Research;

namespace CoiShipMod
{
    internal sealed class SpacecraftPrototypeData : IModData
    {
        private static readonly ResearchNodeProto.ID SpacecraftResearchId =
            new ResearchNodeProto.ID("coishipmod_spacecraft_research");

        private static readonly ResearchNodeProto.ID CargoShipResearchId =
            new ResearchNodeProto.ID("coishipmod_cargo_ship_research");

        private static readonly ResearchNodeProto.ID ScienceShipResearchId =
            new ResearchNodeProto.ID("coishipmod_science_ship_research");

        private static readonly ResearchNodeProto.ID MilitaryShipResearchId =
            new ResearchNodeProto.ID("coishipmod_military_ship_research");

        private static readonly StaticEntityProto.ID SpacecraftAssemblyId =
            new StaticEntityProto.ID("coishipmod_spacecraft_assembly");

        private static readonly StaticEntityProto.ID CargoShipId =
            new StaticEntityProto.ID("coishipmod_cargo_ship");

        private static readonly StaticEntityProto.ID ScienceShipId =
            new StaticEntityProto.ID("coishipmod_science_ship");

        private static readonly StaticEntityProto.ID MilitaryShipId =
            new StaticEntityProto.ID("coishipmod_military_ship");

        private static readonly ShipDefinition[] Ships =
        {
            new ShipDefinition(
                "Cargo spatial",
                "Space cargo",
                "Cargo spatial / Space cargo",
                8,
                0,
                CargoShipId,
                CargoShipResearchId,
                "Assets/CoiShipMod/Icons/Ui/cargaison.svg",
                new[]
                {
                    new ResourceCost("Fer / Iron", 800),
                    new ResourceCost("Aluminium / Aluminum", 500),
                    new ResourceCost("Pièces de construction / Construction parts", 350),
                    new ResourceCost("Électronique / Electronics", 220),
                    new ResourceCost("Carburant / Fuel", 300)
                }),

            new ShipDefinition(
                "Vaisseau scientifique",
                "Science ship",
                "Vaisseau scientifique / Science ship",
                10,
                1,
                ScienceShipId,
                ScienceShipResearchId,
                "Assets/CoiShipMod/Icons/Ui/recherche.svg",
                new[]
                {
                    new ResourceCost("Fer / Iron", 650),
                    new ResourceCost("Aluminium / Aluminum", 450),
                    new ResourceCost("Électronique / Electronics", 500),
                    new ResourceCost("Cuivre / Copper", 220),
                    new ResourceCost("Carburant / Fuel", 350),
                    new ResourceCost("Pièces de construction / Construction parts", 280)
                }),

            new ShipDefinition(
                "Vaisseau militaire",
                "Military ship",
                "Vaisseau militaire / Military ship",
                12,
                2,
                MilitaryShipId,
                MilitaryShipResearchId,
                "Assets/CoiShipMod/Icons/Ui/armement.svg",
                new[]
                {
                    new ResourceCost("Fer / Iron", 1200),
                    new ResourceCost("Aluminium / Aluminum", 700),
                    new ResourceCost("Électronique / Electronics", 700),
                    new ResourceCost("Cuivre / Copper", 300),
                    new ResourceCost("Carburant / Fuel", 500),
                    new ResourceCost("Pièces de construction / Construction parts", 420)
                })
        };

        public void RegisterData(ProtoRegistrator registrator)
        {
            var database = registrator.PrototypesDb;

            var anchorResearch = database
                .All(typeof(ResearchNodeProto))
                .OfType<ResearchNodeProto>()
                .FirstOrDefault();

            if (anchorResearch == null)
            {
                throw new InvalidOperationException("Aucun nœud de recherche trouvé pour positionner l'arbre des vaisseaux spatiaux.");
            }

            var rocketAssemblyResearch = TryGetRocketAssemblyResearch(database, anchorResearch);

            var rootBuilder = registrator.ResearchNodeProtoBuilder
                .Start(
                    "Vaisseaux spatiaux / Spacecraft",
                    SpacecraftResearchId,
                    costMonths: 10,
                    translationComment: "Débloque l'atelier et les vaisseaux spatiaux.")
                .Description(
                    "Débloque l'atelier de vaisseaux spatiaux, le cargo spatial, le vaisseau scientifique et le vaisseau militaire. / Unlocks the spacecraft assembly plant, cargo ship, science ship, and military ship.",
                    "CoiShipMod")
                .AddIcon("Assets/CoiShipMod/Icons/Ui/vaisseau.svg")
                .SetGridPosition(new Vector2i(anchorResearch.GridPosition.X + 3, anchorResearch.GridPosition.Y));

            if (rocketAssemblyResearch != null)
            {
                rootBuilder.AddParents(rocketAssemblyResearch);
            }

            rootBuilder.AddParents(anchorResearch);

            rootBuilder.BuildAndAdd();

            foreach (var ship in Ships)
            {
                var shipBuilder = registrator.ResearchNodeProtoBuilder
                    .Start(
                        ship.NameFr,
                        ship.ResearchId,
                        costMonths: ship.CostMonths,
                        translationComment: ship.TitleFr)
                    .Description(
                        "Débloque " + ship.NameFr + ". / Unlocks " + ship.NameEn + ".",
                        "CoiShipMod")
                    .AddIcon(ship.IconPath)
                    .AddParents(GetResearchByIdOrFallback(database, SpacecraftResearchId))
                    .SetGridPosition(GetChildPosition(anchorResearch, ship.GridOffset));

                shipBuilder.BuildAndAdd();
            }
        }

        private static ResearchNodeProto TryGetRocketAssemblyResearch(ProtosDb database, ResearchNodeProto anchorResearch)
        {
            var candidateIds = new[]
            {
                "rocket_assembly",
                "rocket_launch",
                "rocket_assembly_and_launch",
                "rockets",
                "space_program_rocket_assembly",
                "space_program_rocket_launch",
                "space_program_rocket_assembly_and_launch",
                "rocket",
                "rockets_assembly",
                "rockets_launch",
                "rocket_and_launch",
                "launch_rocket",
                "assembly_and_launch"
            };

            foreach (var id in candidateIds)
            {
                try
                {
                    var node = database.GetOrThrow<ResearchNodeProto>(new ResearchNodeProto.ID(id));
                    if (node != null)
                    {
                        return node;
                    }
                }
                catch
                {
                }
            }

            return anchorResearch;
        }

        private static Vector2i GetChildPosition(ResearchNodeProto anchorResearch, int offsetY)
        {
            return new Vector2i(anchorResearch.GridPosition.X + 5, anchorResearch.GridPosition.Y + offsetY);
        }

        private static ResearchNodeProto GetResearchByIdOrFallback(ProtosDb database, ResearchNodeProto.ID id)
        {
            return database.GetOrThrow<ResearchNodeProto>(id);
        }

        private sealed class ShipDefinition
        {
            public ShipDefinition(
                string nameFr,
                string nameEn,
                string titleFr,
                int costMonths,
                int gridOffset,
                StaticEntityProto.ID entityId,
                ResearchNodeProto.ID researchId,
                string iconPath,
                ResourceCost[] resourceCosts)
            {
                NameFr = nameFr;
                NameEn = nameEn;
                TitleFr = titleFr;
                CostMonths = costMonths;
                GridOffset = gridOffset;
                EntityId = entityId;
                ResearchId = researchId;
                IconPath = iconPath;
                ResourceCosts = resourceCosts;
            }

            public string NameFr { get; }
            public string NameEn { get; }
            public string TitleFr { get; }
            public int CostMonths { get; }
            public int GridOffset { get; }
            public StaticEntityProto.ID EntityId { get; }
            public ResearchNodeProto.ID ResearchId { get; }
            public string IconPath { get; }
            public ResourceCost[] ResourceCosts { get; }
        }

        private sealed class ResourceCost
        {
            public ResourceCost(string displayName, int amount)
            {
                DisplayName = displayName;
                Amount = amount;
            }

            public string DisplayName { get; }
            public int Amount { get; }
        }
    }
}
