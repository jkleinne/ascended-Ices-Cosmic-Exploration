using System.Collections.Generic;

namespace ICE.Utilities;

public static unsafe class GatheringUtil
{

    public class GatheringActions
    {
        /// <summary>
        /// Internal name for myself to know wtf this is
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// Sheet name
        /// </summary>
        public string BtnName { get; set; }
        /// <summary>
        /// Botanist Action ID
        /// </summary>
        public uint BtnActionId { get; set; }
        /// <summary>
        /// Sheet name
        /// </summary>
        public string MinName { get; set; }
        /// <summary>
        /// Miner Action ID
        /// </summary>
        public uint MinActionId { get; set; }
        /// <summary>
        /// If it has a status, the ID associated with it
        /// </summary>
        public uint StatusId { get; set; }
        /// <summary>
        /// The status name attached to it (personal use)
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// The amount of GP required for the skill
        /// </summary>
        public int RequiredGp { get; set; }
    }

    public static Dictionary<string, GatheringActions> GathActionDict = new()
    {
        { "BoonIncrease1", new GatheringActions
        {
            ActionName = "Pioneer's Gift I",
            BtnName = "",
            BtnActionId = 21178,
            MinName = "",
            MinActionId = 21177,
            StatusId = 2666,
            StatusName = "Gift of the Land",
            RequiredGp = 50,
        }},
        { "BoonIncrease2", new GatheringActions
        {
            ActionName = "Pioneer's Gift II",
            BtnName = "",
            BtnActionId = 25590,
            MinName = "",
            MinActionId = 25589,
            StatusId = 759,
            StatusName = "Gift of the Land II",
            RequiredGp = 100,
        }},
        { "Tidings", new GatheringActions
        {
            ActionName = "Nophica's Tidings",
            BtnName = "",
            BtnActionId = 21204,
            MinName = "",
            MinActionId = 21203,
            StatusId = 2667,
            StatusName = "Gatherer's Bounty",
            RequiredGp = 200,
        }},
        { "YieldI", new GatheringActions
        {
            ActionName = "Blessed Harvest",
            BtnName = "",
            BtnActionId = 222,
            MinName = "",
            MinActionId = 239,
            StatusId = 219,
            StatusName = "Gathering Yield Up",
            RequiredGp = 400,
        }},
        { "YieldII", new GatheringActions
        {
            ActionName = "Blessed Harvest II",
            BtnName = "",
            BtnActionId = 224,
            MinName = "",
            MinActionId = 241,
            StatusId = 219,
            StatusName = "Gathering Yield Up",
            RequiredGp = 500,
        }},
        { "IntegrityIncrease", new GatheringActions
        {
            ActionName = "Ageless Words",
            BtnName = "",
            BtnActionId = 215,
            MinName = "",
            MinActionId = 232,
            RequiredGp = 300,
        }},
        { "BonusIntegrityChance", new GatheringActions
        {
            ActionName = "Wise of the World",
            BtnName = "",
            BtnActionId = 26522,
            MinName = "",
            MinActionId = 26521,
            StatusId = 2765,
            StatusName = "",
            RequiredGp = 0,
        }},
        { "BountifulYieldII", new GatheringActions
        {
            ActionName = "Bountiful Yield/Harvest II",
            BtnName = "",
            BtnActionId = 273,
            MinName = "",
            MinActionId = 272,
            StatusId = 1286,
            StatusName = "",
            RequiredGp = 100,
        }},
    };

    /* First things first, there's several types of missions for gathering
     * 1 Quantity Limited(Gather x amount on limited amount of nodes)
     * 2 Quantity(Gather x amount, gather more for increased score)
     * 3 Timed(Gather x amount in the time limit)
     * 4 Chain(Increase score based on chain)
     * 5 Gatherer's Boon (Increase score by hitting boon % chance)
     * 6 Chain + Boon(Get score from chain nodes + boon % chance)
     * 7 Collectables(This is going to be annoying)
     * 8 Time Steller Reduction(???) (Assuming Collectables -> Reducing for score...fuck)
     */

    public static Dictionary<Vector2, uint> OldNodeset = new()
    {
        // Miner Set
        { new Vector2(-119, -175), 1 },
        { new Vector2(-168, -181), 2 },
        { new Vector2(96, 259), 3 },
        { new Vector2(65, -431), 4 },
        { new Vector2(-475, 135),  5 },
        { new Vector2(73, -482), 6 },
        { new Vector2(-463, -729), 7 },
        { new Vector2(-690, -752), 8 },
        { new Vector2(-669, -515), 9 },

        // Botanist Set
        { new Vector2(-278, -13), 11 },
        { new Vector2(225, 83), 12 },
        { new Vector2(232, -50), 13 },
        { new Vector2(456, 221), 14 },
        { new Vector2(-121, 368), 15 },
        { new Vector2(455, 243), 16 },
        { new Vector2(506, 682), 17 },
        { new Vector2(609, 478), 18 },
        { new Vector2(527, 630), 19 },
        { new Vector2(-706, 564), 20 },

        // Criticals -- Placeholders
        { new Vector2(-503, -324), 21 },
        { new Vector2(-131, -365), 22 },
        { new Vector2(-270, 140), 23 },
        // { new Vector2(566, -908), 24 },
        { new Vector2(188, -201), 25 },
        { new Vector2(748, 101), 26 },
    };

    public class GathNodeInfo
    {
        public Vector3 Position { get; set; }
        public Vector3 LandZone { get; set; }
        public uint NodeId { get; set; }
        public int GatheringType { get; set; }
        public int ZoneId { get; set; }
        public uint NodeSet { get; set; }
    }

    public class FisherSpotInfo
    {
        public Vector3 NavtoSpot { get; set; }
        public Vector3 FishingSpot { get; set; }
    }

    public static Dictionary<Vector2, List<FisherSpotInfo>> FishingLocation = new()
    {
        { new Vector2(104, -269), new List<FisherSpotInfo>() {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(67.09f, 19.13f, -246.34f),
                FishingSpot = new Vector3(72.78f, 18.07f, -251.65f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(81.34f, 18.19f, -232.67f),
                FishingSpot = new Vector3(86.95f, 18.50f, -236.47f)
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(112.55f, 19.17f, -226.95f),
                FishingSpot = new Vector3(110.03f, 18.81f, -229.61f)
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(136.14f, 22.61f, -235.72f),
                FishingSpot = new Vector3(132.63f, 22.12f, -238.54f)
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(146.02f, 28.76f, -281.85f),
                FishingSpot = new Vector3(143.39f, 28.23f, -279.23f)
            },
        } },
        { new Vector2(-139, -283), new List<FisherSpotInfo>(){
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-119.67f, 20.40f, -285.04f),
                FishingSpot = new Vector3(-124.69f, 20.12f, -286.35f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-116.36f, 21.45f, -303.67f),
                FishingSpot = new Vector3(-119.45f, 21.23f, -302.83f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-132.22f, 22.14f, -315.09f),
                FishingSpot = new Vector3(-133.92f, 21.89f, -307.77f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-149.69f, 21.98f, -303.55f),
                FishingSpot = new Vector3(-146.28f, 21.34f, -301.40f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-157.92f, 21.41f, -274.88f),
                FishingSpot = new Vector3(-153.74f, 20.53f, -275.38f),
            }
        } },
        { new Vector2(-281, -104), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-263.24f, 26.04f, -123.94f),
                FishingSpot = new Vector3(-266.38f, 25.28f, -122.38f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-261.11f, 24.92f, -111.89f),
                FishingSpot = new Vector3(-262.08f, 24.88f, -111.59f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-255.11f, 22.12f, -86.71f),
                FishingSpot = new Vector3(-257.40f, 21.89f, -86.17f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-257.32f, 23.20f, -73.09f),
                FishingSpot = new Vector3(-258.54f, 22.30f, -76.33f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-286.72f, 23.03f, -70.65f),
                FishingSpot = new Vector3(-285.15f, 22.29f, -72.24f),
            },
        } },
        { new Vector2(193, 196), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(209.92f, 19.74f, 205.66f),
                FishingSpot = new Vector3(207.89f, 19.52f, 203.26f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(201.62f, 19.74f, 210.88f),
                FishingSpot = new Vector3(200.33f, 19.44f, 208.05f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(192.16f, 19.16f, 208.58f),
                FishingSpot = new Vector3(192.16f, 19.16f, 208.58f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(174.96f, 18.57f, 194.23f),
                FishingSpot = new Vector3(177.16f, 17.96f, 194.73f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(190.70f, 18.08f, 182.53f),
                FishingSpot = new Vector3(191.45f, 17.58f, 186.35f),
            },
        } },
        { new Vector2(573, 573), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(512.46f, 52.25f, 568.22f),
                FishingSpot = new Vector3(516.74f, 52.35f, 568.17f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(560.55f, 51.35f, 528.83f),
                FishingSpot = new Vector3(562.74f, 50.80f, 533.07f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(588.83f, 50.18f, 488.87f),
                FishingSpot = new Vector3(591.27f, 49.92f, 491.14f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(626.51f, 54.19f, 528.09f),
                FishingSpot = new Vector3(623.40f, 54.05f, 529.16f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(614.27f, 54.74f, 583.04f),
                FishingSpot = new Vector3(611.00f, 54.41f, 580.93f),
            },
        } },
        { new Vector2(-642, -631), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-683.40f, 75.44f, -602.75f),
                FishingSpot = new Vector3(-684.47f, 74.90f, -599.87f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-696.28f, 78.40f, -613.17f),
                FishingSpot = new Vector3(-697.79f, 78.65f, -611.57f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-705.91f, 80.24f, -640.99f),
                FishingSpot = new Vector3(-709.01f, 80.49f, -640.24f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-717.26f, 81.34f, -680.15f),
                FishingSpot = new Vector3(-717.43f, 81.14f, -676.65f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-751.98f, 83.22f, -657.03f),
                FishingSpot = new Vector3(-749.07f, 83.04f, -655.83f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-592.44f, 69.78f, -675.48f),
                FishingSpot = new Vector3(-589.42f, 69.64f, -675.19f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-582.07f, 70.15f, -702.85f),
                FishingSpot = new Vector3(-579.83f, 69.57f, -700.20f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-562.54f, 69.07f, -706.32f),
                FishingSpot = new Vector3(-563.15f, 68.70f, -703.20f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-531.57f, 68.99f, -686.71f),
                FishingSpot = new Vector3(-534.03f, 69.17f, -685.55f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-532.27f, 68.40f, -670.41f),
                FishingSpot = new Vector3(-535.46f, 69.00f, -671.25f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-557.52f, 68.54f, -646.11f),
                FishingSpot = new Vector3(-558.69f, 68.80f, -649.56f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-586.18f, 67.78f, -637.35f),
                FishingSpot = new Vector3(-584.33f, 68.06f, -640.09f),
            },
        } },
        { new Vector2(909, -336), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(914.64f, -58.02f, -324.31f),
                FishingSpot = new Vector3(917.44f, -58.05f, -323.82f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(916.67f, -58.75f, -337.77f),
                FishingSpot = new Vector3(918.65f, -58.89f, -337.07f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(920.99f, -58.76f, -351.82f),
                FishingSpot = new Vector3(921.87f, -58.69f, -350.27f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(927.40f, -58.56f, -356.18f),
                FishingSpot = new Vector3(928.81f, -58.54f, -355.07f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(934.18f, -57.12f, -361.69f),
                FishingSpot = new Vector3(935.86f, -56.89f, -360.24f),
            },
        } },
        { new Vector2(-673, 497), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-737.19f, 69.92f, 509.45f),
                FishingSpot = new Vector3(-742.38f, 69.99f, 511.53f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-756.84f, 70.81f, 486.75f),
                FishingSpot = new Vector3(-757.83f, 70.11f, 490.82f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-717.75f, 57.43f, 438.85f),
                FishingSpot = new Vector3(-713.94f, 57.14f, 437.88f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-721.22f, 57.21f, 409.58f),
                FishingSpot = new Vector3(-716.97f, 57.09f, 410.51f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-609.42f, 50.00f, 515.78f),
                FishingSpot = new Vector3(-607.97f, 49.84f, 519.56f),
            },
        } },
        { new Vector2(-348, 604), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-381.03f, 47.83f, 590.70f),
                FishingSpot = new Vector3(-376.01f, 47.63f, 591.75f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-372.27f, 47.92f, 572.60f),
                FishingSpot = new Vector3(-369.26f, 47.70f, 574.86f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-341.73f, 47.75f, 563.59f),
                FishingSpot = new Vector3(-342.58f, 47.59f, 567.20f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-320.82f, 47.78f, 579.43f),
                FishingSpot = new Vector3(-322.84f, 47.60f, 580.93f),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(-324.11f, 47.83f, 604.22f),
                FishingSpot = new Vector3(-327.31f, 47.65f, 603.05f),
            },
        } },
        { new Vector2(0, 0), new List<FisherSpotInfo>()
        {
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(),
                FishingSpot = new Vector3(),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(),
                FishingSpot = new Vector3(),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(),
                FishingSpot = new Vector3(),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(),
                FishingSpot = new Vector3(),
            },
            new FisherSpotInfo
            {
                NavtoSpot = new Vector3(),
                FishingSpot = new Vector3(),
            },
        } },
    };

    public static Dictionary<Vector2, List<GathNodeInfo>> GatheringLocation = new()
    {
        // - - - - - - - - - - - 
        // Miner
        // - - - - - - - - - - - 

        // Miner | 8 nodes | #1
        { new Vector2(-119, -175), new List<GathNodeInfo>() {
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35040,
                    Position = new Vector3 (-50.68f, 19.41f, -208.97f),
                    LandZone = new Vector3 (-50.64f, 18.41f, -209.93f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35039,
                    Position = new Vector3 (-73.1f, 20.16f, -204.29f),
                    LandZone = new Vector3 (-73.45f, 19.12f, -205.25f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35038,
                    Position = new Vector3 (-90.95f, 22.52f, -194.11f),
                    LandZone = new Vector3 (-91.03f, 21.15f, -194.67f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35037,
                    Position = new Vector3 (-109.77f, 24.82f, -187.37f),
                    LandZone = new Vector3 (-109.83f, 23.74f, -188.08f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35036,
                    Position = new Vector3 (-129.53f, 27.94f, -170.34f),
                    LandZone = new Vector3 (-129.85f, 26.59f, -170.81f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35035,
                    Position = new Vector3 (-135.65f, 28.2f, -156.82f),
                    LandZone = new Vector3 (-135.94f, 27f, -157.07f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35034,
                    Position = new Vector3 (-142.32f, 27.14f, -139.7f),
                    LandZone = new Vector3 (-142.57f, 25.67f, -140.04f),
                    GatheringType = 2,
                    NodeSet = 1
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35033,
                    Position = new Vector3 (-153.94f, 24.31f, -124.47f),
                    LandZone = new Vector3 (-154.36f, 23.29f, -124.61f),
                    GatheringType = 2,
                    NodeSet = 1
                },
        } },
        // Miner | 6 Nodes | #2
        { new Vector2(-168, -181), new List<GathNodeInfo>() {
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35062,
                    Position = new Vector3 (-110.86f, 20.37f, -226.34f),
                    LandZone = new Vector3 (-110.73f, 19.3f, -225.31f),
                    GatheringType = 2,
                    NodeSet = 2
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35061,
                    Position = new Vector3 (-117.08f, 20.9f, -230.27f),
                    LandZone = new Vector3 (-118.01f, 20f, -230.48f),
                    GatheringType = 2,
                    NodeSet = 2
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35057,
                    Position = new Vector3 (-226.12f, 29.29f, -176.71f),
                    LandZone = new Vector3 (-225.34f, 28.75f, -176.68f),
                    GatheringType = 2,
                    NodeSet = 2
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35058,
                    Position = new Vector3 (-228.37f, 27.66f, -167.48f),
                    LandZone = new Vector3 (-227.69f, 27.39f, -167.57f),
                    GatheringType = 2,
                    NodeSet = 2
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35059,
                    Position = new Vector3 (-170.4f, 20.94f, -116.41f),
                    LandZone = new Vector3 (-170.99f, 20.28f, -116.77f),
                    GatheringType = 2,
                    NodeSet = 2
                },
            new GathNodeInfo
                {
                    ZoneId = 1237,
                    NodeId = 35060,
                    Position = new Vector3 (-162.69f, 22.16f, -124.16f),
                    LandZone = new Vector3 (-162.84f, 21.93f, -125.09f),
                    GatheringType = 2,
                    NodeSet = 2
                },
        } },
        // Miner | 6 Nodes | #3
        { new Vector2(96, 259), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35066,
                Position = new Vector3 (60.94f, 22.63f, 204.94f),
                LandZone = new Vector3 (60.95f, 21.53f, 205.38f),
                GatheringType = 2,
                NodeSet = 3
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35065,
                Position = new Vector3 (40.95f, 21.06f, 208.7f),
                LandZone = new Vector3 (41.39f, 19.97f, 209.17f),
                GatheringType = 2,
                NodeSet = 3
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35063,
                Position = new Vector3 (65.52f, 18.94f, 318.76f),
                LandZone = new Vector3 (65.6f, 18.21f, 318.18f),
                GatheringType = 2,
                NodeSet = 3
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35064,
                Position = new Vector3 (75.12f, 19.39f, 322.87f),
                LandZone = new Vector3 (75.45f, 18.57f, 322.06f),
                GatheringType = 2,
                NodeSet = 3
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35068,
                Position = new Vector3 (172.32f, 23.64f, 275.08f),
                LandZone = new Vector3 (172.16f, 22.95f, 274.95f),
                GatheringType = 2,
                NodeSet = 3
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35067,
                Position = new Vector3 (174.37f, 23.41f, 267.84f),
                LandZone = new Vector3 (174.24f, 22.82f, 267.61f),
                GatheringType = 2,
                NodeSet = 3
            },
        } },
        // Miner | 8 Nodes | #4
        { new Vector2(65, -431), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35041,
                Position = new Vector3 (70.23f, 35.63f, -370.83f),
                LandZone = new Vector3 (69.72f, 35.02f, -370.42f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35042,
                Position = new Vector3 (56.98f, 36.75f, -385.37f),
                LandZone = new Vector3 (57.49f, 35.95f, -384.87f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35043,
                Position = new Vector3 (78.47f, 39.5f, -424.33f),
                LandZone = new Vector3 (77.97f, 39.06f, -424.98f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35044,
                Position = new Vector3 (56.85f, 40.02f, -444.27f),
                LandZone = new Vector3 (57.26f, 39.27f, -444f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35045,
                Position = new Vector3 (56.59f, 40.24f, -453.96f),
                LandZone = new Vector3 (57.03f, 39.6f, -454.24f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35046,
                Position = new Vector3 (45.23f, 41.43f, -473.65f),
                LandZone = new Vector3 (45.55f, 40.41f, -473.4f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35047,
                Position = new Vector3 (49.7f, 41.83f, -481.26f),
                LandZone = new Vector3 (50.06f, 40.96f, -481.57f),
                GatheringType = 2,
                NodeSet = 4
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35048,
                Position = new Vector3 (60.49f, 43.1f, -499.58f),
                LandZone = new Vector3 (60.52f, 42.33f, -499.64f),
                GatheringType = 2,
                NodeSet = 4
            },
        } },
        // Miner | 6 Nodes | #5
        { new Vector2(-475, 135), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35072,
                Position = new Vector3 (-430.47f, 42.53f, 96.48f),
                LandZone = new Vector3 (-430.96f, 42.04f, 96.05f),
                GatheringType = 2,
                NodeSet = 5
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35071,
                Position = new Vector3 (-438.91f, 43.87f, 101.09f),
                LandZone = new Vector3 (-438.25f, 42.71f, 101.14f),
                GatheringType = 2,
                NodeSet = 5
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35074,
                Position = new Vector3 (-543.83f, 41.39f, 78.79f),
                LandZone = new Vector3 (-543.35f, 40.24f, 78.7f),
                GatheringType = 2,
                NodeSet = 5
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35073,
                Position = new Vector3 (-542.48f, 44.36f, 91.48f),
                LandZone = new Vector3 (-542.08f, 43.15f, 91.97f),
                GatheringType = 2,
                NodeSet = 5
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35070,
                Position = new Vector3 (-400.24f, 45f, 191.3f),
                LandZone = new Vector3 (-400.94f, 44.26f, 190.28f),
                GatheringType = 2,
                NodeSet = 5
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35069,
                Position = new Vector3 (-394.25f, 44.13f, 189.71f),
                LandZone = new Vector3 (-394.35f, 43.28f, 188.94f),
                GatheringType = 2,
                NodeSet = 5
            },
        } },
        // Miner | 6 Nodes | #6
        { new Vector2(73, -482), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35078,
                Position = new Vector3 (77.56f, 39.48f, -424f),
                LandZone = new Vector3 (76.97f, 38.91f, -424.55f),
                GatheringType = 2,
                NodeSet = 6
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35077,
                Position = new Vector3 (87.1f, 39.77f, -424.88f),
                LandZone = new Vector3 (87.04f, 39.23f, -425.47f),
                GatheringType = 2,
                NodeSet = 6
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35076,
                Position = new Vector3 (142.59f, 47.46f, -491.28f),
                LandZone = new Vector3 (142.78f, 46.57f, -490.84f),
                GatheringType = 2,
                NodeSet = 6
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35075,
                Position = new Vector3 (135.91f, 47.36f, -500.54f),
                LandZone = new Vector3 (135.63f, 46.7f, -500.5f),
                GatheringType = 2,
                NodeSet = 6
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35079,
                Position = new Vector3 (32.70f, 43.50f, -521.08f),
                LandZone = new Vector3 (32.70f, 43.50f, -521.08f),
                GatheringType = 2,
                NodeSet = 6
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35080,
                Position = new Vector3 (15.94f, 44.39f, -526.71f),
                LandZone = new Vector3 (15.66f, 43.36f, -526.49f),
                GatheringType = 2,
                NodeSet = 6
            },
        } },
        // Miner | 8 Nodes | #7
        { new Vector2(-463, -729), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35056,
                Position = new Vector3 (-419.94f, 66.8f, -692.3f),
                LandZone = new Vector3 (-420.07f, 66.15f, -691.43f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35055,
                Position = new Vector3 (-428.39f, 67.89f, -704.01f),
                LandZone = new Vector3 (-428.84f, 67.15f, -703.4f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35054,
                Position = new Vector3 (-447.28f, 68.51f, -707.15f),
                LandZone = new Vector3 (-446.41f, 67.61f, -707.17f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35053,
                Position = new Vector3 (-461.66f, 69.71f, -713.83f),
                LandZone = new Vector3 (-462.18f, 68.99f, -713.69f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35052,
                Position = new Vector3 (-462.91f, 71.27f, -731.6f),
                LandZone = new Vector3 (-463.43f, 70.55f, -731.26f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35051,
                Position = new Vector3 (-467.54f, 73.48f, -747.74f),
                LandZone = new Vector3 (-468f, 72.64f, -747.89f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35050,
                Position = new Vector3 (-469.04f, 76.77f, -770.29f),
                LandZone = new Vector3 (-469.29f, 76.02f, -769.88f),
                GatheringType = 2,
                NodeSet = 7
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35049,
                Position = new Vector3 (-492.64f, 78.8f, -777f),
                LandZone = new Vector3 (-492.64f, 78.01f, -776.5f),
                GatheringType = 2,
                NodeSet = 7
            },
        } },
        // Miner | 6 Nodes | #8
        { new Vector2(-690, -752), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35086,
                Position = new Vector3 (-635.22f, 73.97f, -704.67f),
                LandZone = new Vector3 (-636.22f, 73.17f, -703.98f),
                GatheringType = 2,
                NodeSet = 8
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35085,
                Position = new Vector3 (-621.59f, 75.08f, -715.89f),
                LandZone = new Vector3 (-620.05f, 74.21f, -717.71f),
                GatheringType = 2,
                NodeSet = 8
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35084,
                Position = new Vector3 (-671.18f, 93.37f, -819.39f),
                LandZone = new Vector3 (-670.57f, 92.57f, -819.02f),
                GatheringType = 2,
                NodeSet = 8
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35083,
                Position = new Vector3 (-679.34f, 91.67f, -804.68f),
                LandZone = new Vector3 (-678.57f, 90.89f, -804.33f),
                GatheringType = 2,
                NodeSet = 8
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35082,
                Position = new Vector3 (-752.37f, 88.51f, -717.92f),
                LandZone = new Vector3 (-751.92f, 87.59f, -717.87f),
                GatheringType = 2,
                NodeSet = 8
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35081,
                Position = new Vector3 (-758.07f, 88.73f, -707.39f),
                LandZone = new Vector3 (-757.45f, 87.93f, -707.14f),
                GatheringType = 2,
                NodeSet = 8
            },
        } },
        // Miner | x Nodes | #9
        { new Vector2(-669, -515), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35091,
                Position = new Vector3 (-642.11f, 69.73f, -572.83f),
                LandZone = new Vector3 (-641.41f, 68.81f, -572.6f),
                GatheringType = 2,
                NodeSet = 9
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35092,
                Position = new Vector3 (-652.4f, 71.72f, -564.69f),
                LandZone = new Vector3 (-652.33f, 70.89f, -564.22f),
                GatheringType = 2,
                NodeSet = 9
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35089,
                Position = new Vector3 (-731.63f, 79.66f, -509.58f),
                LandZone = new Vector3 (-731.27f, 78.7f, -509.72f),
                GatheringType = 2,
                NodeSet = 9
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35090,
                Position = new Vector3 (-727.81f, 79.13f, -503.01f),
                LandZone = new Vector3 (-727.68f, 78.05f, -502.8f),
                GatheringType = 2,
                NodeSet = 9
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35087,
                Position = new Vector3 (-640.96f, 60.56f, -463.86f),
                LandZone = new Vector3 (-640.66f, 59.77f, -463.8f),
                GatheringType = 2,
                NodeSet = 9
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35088,
                Position = new Vector3 (-637.53f, 59.94f, -456.5f),
                LandZone = new Vector3 (-637.1f, 58.91f, -456.96f),
                GatheringType = 2,
                NodeSet = 9
            },
        } },

        // Criticals -
        // Miner | Critial #1
        { new Vector2(-503, -324), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35115,
                Position = new Vector3(-501.869f, 48.78254f, -376.7437f),
                LandZone = new Vector3(-501.869f, 48.78254f, -376.7437f),
                GatheringType = 2,
                NodeSet = 21
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35116,
                Position = new Vector3(-518.8838f, 48.09315f, -370.4776f),
                LandZone = new Vector3(-518.8838f, 48.09315f, -370.4776f),
                GatheringType = 2,
                NodeSet = 21
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35112,
                Position = new Vector3(-541.9754f, 48.69396f, -303.8165f),
                LandZone = new Vector3(-541.9754f, 48.69396f, -303.8165f),
                GatheringType = 2,
                NodeSet = 21
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35111,
                Position = new Vector3(-544.8497f, 46.38572f, -288.653f),
                LandZone = new Vector3(-544.8497f, 46.38572f, -288.653f),
                GatheringType = 2,
                NodeSet = 21
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35114,
                Position = new Vector3(-453.9398f, 43.02265f, -287.6784f),
                LandZone = new Vector3(-453.9398f, 43.02265f, -287.6784f),
                GatheringType = 2,
                NodeSet = 21
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35113,
                Position = new Vector3(-459.9001f, 44.85727f, -304.8606f),
                LandZone = new Vector3(-459.9001f, 44.85727f, -304.8606f),
                GatheringType = 2,
                NodeSet = 21
            },
        } },
        // Miner | Critial #2
        { new Vector2(-131, -365), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35118,
                Position = new Vector3(-88.08077f, 22.78399f, -356.0785f),
                LandZone = new Vector3(-88.08077f, 22.78399f, -356.0785f),
                GatheringType = 2,
                NodeSet = 22
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35117,
                Position = new Vector3(-91.31516f, 22.02787f, -345.187f),
                LandZone = new Vector3(-91.31516f, 22.02787f, -345.187f),
                GatheringType = 2,
                NodeSet = 22
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35119,
                Position = new Vector3(-127.8303f, 30.40603f, -423.2329f),
                LandZone = new Vector3(-127.8303f, 30.40603f, -423.2329f),
                GatheringType = 2,
                NodeSet = 22
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35120,
                Position = new Vector3(-146.9514f, 29.88647f, -412.1004f),
                LandZone = new Vector3(-146.9514f, 29.88647f, -412.1004f),
                GatheringType = 2,
                NodeSet = 22
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35121,
                Position = new Vector3(-160.7067f, 25.29499f, -317.5167f),
                LandZone = new Vector3(-160.7067f, 25.29499f, -317.5167f),
                GatheringType = 2,
                NodeSet = 22
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35122,
                Position = new Vector3(-150.4397f, 23.76383f, -313.2321f),
                LandZone = new Vector3(-150.4397f, 23.76383f, -313.2321f),
                GatheringType = 2,
                NodeSet = 22
            },
        } },
        // Miner | Critical #3
        { new Vector2(-270, 140), new List < GathNodeInfo >() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35132,
                Position = new Vector3(-314.1873f, 27.93501f, 121.8422f),
                LandZone = new Vector3(-314.1873f, 27.93501f, 121.8422f),
                GatheringType = 2,
                NodeSet = 23
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35133,
                Position = new Vector3(-317.8292f, 28.43479f, 135.2601f),
                LandZone = new Vector3(-317.8292f, 28.43479f, 135.2601f),
                GatheringType = 2,
                NodeSet = 23
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35134,
                Position = new Vector3(-277.699f, 24.79578f, 187.4265f),
                LandZone = new Vector3(-277.699f, 24.79578f, 187.4265f),
                GatheringType = 2,
                NodeSet = 23
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35131,
                Position = new Vector3(-245.4429f, 24.62878f, 188.8734f),
                LandZone = new Vector3(-245.4429f, 24.62878f, 188.8734f),
                GatheringType = 2,
                NodeSet = 23
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35129,
                Position = new Vector3(-216.6629f, 19.97406f, 105.9128f),
                LandZone = new Vector3(-216.6629f, 19.97406f, 105.9128f),
                GatheringType = 2,
                NodeSet = 23
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35130,
                Position = new Vector3(-243.6713f, 20.00452f, 90.37915f),
                LandZone = new Vector3(-243.6713f, 20.00452f, 90.37915f),
                GatheringType = 2,
                NodeSet = 23
            },
        } },

        // - - - - - - - - - - 
        // Botanist
        // - - - - - - - - - - 

        // Botanist | 6 Nodes | #1
        { new Vector2(-278, -13), new List<GathNodeInfo>(){
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35168,
                Position = new Vector3 (-228.04f, 17.31f, -13.59f),
                LandZone = new Vector3 (-228.84f, 16.82f, -13.53f),
                GatheringType = 3,
                NodeSet = 11
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35167,
                Position = new Vector3 (-238.16f, 17.82f, -33.92f),
                LandZone = new Vector3 (-237.94f, 17.22f, -33.02f),
                GatheringType = 3,
                NodeSet = 11
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35166,
                Position = new Vector3 (-325.44f, 28.14f, -51.83f),
                LandZone = new Vector3 (-324.69f, 27.81f, -51.55f),
                GatheringType = 3,
                NodeSet = 11
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35165,
                Position = new Vector3 (-330.45f, 29.18f, -28.82f),
                LandZone = new Vector3 (-330.08f, 28.69f, -29.41f),
                GatheringType = 3,
                NodeSet = 11
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35169,
                Position = new Vector3 (-288.95f, 25.49f, 44.38f),
                LandZone = new Vector3 (-288.93f, 25.07f, 43.86f),
                GatheringType = 3,
                NodeSet = 11
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35170,
                Position = new Vector3 (-273.81f, 23.78f, 44.75f),
                LandZone = new Vector3 (-274.15f, 23.28f, 44.57f),
                GatheringType = 3,
                NodeSet = 11
            },
        } },
        // Botanist | 6 Nodes | #2
        { new Vector2(225, 83), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35159,
                Position = new Vector3 (223.44f, 20.26f, 8.42f),
                LandZone = new Vector3 (223.04f, 19.41f, 8.25f),
                GatheringType = 3,
                NodeSet = 12
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35160,
                Position = new Vector3 (223.99f, 20.31f, 15.83f),
                LandZone = new Vector3 (224.33f, 19.39f, 15.45f),
                GatheringType = 3,
                NodeSet = 12
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35163,
                Position = new Vector3 (285.3f, 18.88f, 108.14f),
                LandZone = new Vector3 (285.2f, 17.89f, 108.78f),
                GatheringType = 3,
                NodeSet = 12
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35164,
                Position = new Vector3 (286.34f, 19.03f, 114.67f),
                LandZone = new Vector3 (286.04f, 18.02f, 114.53f),
                GatheringType = 3,
                NodeSet = 12
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35162,
                Position = new Vector3 (207.68f, 18.92f, 145.46f),
                LandZone = new Vector3 (207.74f, 17.93f, 145.63f),
                GatheringType = 3,
                NodeSet = 12
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35161,
                Position = new Vector3 (186.24f, 19.58f, 152.27f),
                LandZone = new Vector3 (186.34f, 18.68f, 152.56f),
                GatheringType = 3,
                NodeSet = 12
            },
        } },
        // Botanist | 8 Nodes | #3
        { new Vector2(232, -50), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35137,
                Position = new Vector3 (221.91f, 20.14f, 1.22f),
                LandZone = new Vector3 (221.63f, 19.27f, 1.76f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35136,
                Position = new Vector3 (214.32f, 20.07f, -9.12f),
                LandZone = new Vector3 (214.58f, 19.17f, -9.09f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35141,
                Position = new Vector3 (212.42f, 18.82f, -23.42f),
                LandZone = new Vector3 (212.86f, 18.07f, -22.78f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35140,
                Position = new Vector3 (231.74f, 19.9f, -27.72f),
                LandZone = new Vector3 (231.49f, 18.96f, -27.89f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35139,
                Position = new Vector3 (244.55f, 21.27f, -39.45f),
                LandZone = new Vector3 (244.63f, 20.4f, -39.56f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35138,
                Position = new Vector3 (251.86f, 22.5f, -54.28f),
                LandZone = new Vector3 (252.26f, 21.65f, -54.21f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35142,
                Position = new Vector3 (238.31f, 21.42f, -64.92f),
                LandZone = new Vector3 (238.52f, 20.47f, -65.07f),
                GatheringType = 3,
                NodeSet = 13
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35135,
                Position = new Vector3 (225.6f, 20.73f, -83.05f),
                LandZone = new Vector3 (225.59f, 19.73f, -82.86f),
                GatheringType = 3,
                NodeSet = 13
            },
        } },
        // Botanist | 8 Nodes | #4 
        { new Vector2(456, 221), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35143,
                Position = new Vector3 (421.1f, 33.1f, 189.18f),
                LandZone = new Vector3 (421.21f, 32.62f, 189.4f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35150,
                Position = new Vector3 (441.48f, 33.91f, 186.57f),
                LandZone = new Vector3 (441.11f, 33.22f, 186.48f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35147,
                Position = new Vector3 (439.68f, 34.32f, 211f),
                LandZone = new Vector3 (439.92f, 33.71f, 210.52f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35148,
                Position = new Vector3 (460.41f, 34.31f, 206.39f),
                LandZone = new Vector3 (460.12f, 33.59f, 206.25f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35149,
                Position = new Vector3 (468.54f, 34.92f, 216.16f),
                LandZone = new Vector3 (468.2f, 34.34f, 216.77f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35144,
                Position = new Vector3 (459.23f, 34.89f, 234.63f),
                LandZone = new Vector3 (459.71f, 34.33f, 234.41f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35145,
                Position = new Vector3 (454.67f, 34.75f, 254.14f),
                LandZone = new Vector3 (454.88f, 34.04f, 254.43f),
                GatheringType = 3,
                NodeSet = 14
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35146,
                Position = new Vector3 (461.14f, 35.08f, 268.28f),
                LandZone = new Vector3 (461.07f, 34.47f, 267.78f),
                GatheringType = 3,
                NodeSet = 14
            },
        } },
        // Botanist | 6 Nodes | #5 
        { new Vector2(-121, 368), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35176,
                Position = new Vector3 (-185.84f, 32.88f, 351.45f),
                LandZone = new Vector3 (-185.52f, 32.05f, 351.6f),
                GatheringType = 3,
                NodeSet = 15
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35175,
                Position = new Vector3 (-174.21f, 32.41f, 342.16f),
                LandZone = new Vector3 (-174.13f, 31.51f, 342.22f),
                GatheringType = 3,
                NodeSet = 15
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35171,
                Position = new Vector3 (-83.13f, 28.26f, 312.21f),
                LandZone = new Vector3 (-83.36f, 27.38f, 311.6f),
                GatheringType = 3,
                NodeSet = 15
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35172,
                Position = new Vector3 (-69.83f, 27.94f, 330.99f),
                LandZone = new Vector3 (-69.73f, 27.02f, 330.61f),
                GatheringType = 3,
                NodeSet = 15
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35173,
                Position = new Vector3 (-110.16f, 34.36f, 427.53f),
                LandZone = new Vector3 (-110.27f, 33.45f, 428.06f),
                GatheringType = 3,
                NodeSet = 15
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35174,
                Position = new Vector3 (-116.48f, 34.47f, 438.21f),
                LandZone = new Vector3 (-116.7f, 33.51f, 437.75f),
                GatheringType = 3,
                NodeSet = 15
            },
        } },
        // Botanist | 6 Nodes | #6
        { new Vector2(455, 243), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35177,
                Position = new Vector3 (490.19f, 36.57f, 174.08f),
                LandZone = new Vector3 (489.71f, 35.7f, 174.19f),
                GatheringType = 3,
                NodeSet = 16
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35178,
                Position = new Vector3 (441.94f, 36.48f, 176.07f),
                LandZone = new Vector3 (441.74f, 35.5f, 176.29f),
                GatheringType = 3,
                NodeSet = 16
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35180,
                Position = new Vector3 (382.5f, 35.42f, 265.39f),
                LandZone = new Vector3 (383.06f, 35.01f, 264.97f),
                GatheringType = 3,
                NodeSet = 16
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35179,
                Position = new Vector3 (395.4f, 37.31f, 276.67f),
                LandZone = new Vector3 (395.65f, 36.48f, 276.62f),
                GatheringType = 3,
                NodeSet = 16
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35181,
                Position = new Vector3 (476.83f, 40.68f, 297.1f),
                LandZone = new Vector3 (477.28f, 39.93f, 297.15f),
                GatheringType = 3,
                NodeSet = 16
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35182,
                Position = new Vector3 (510.84f, 37.4f, 283.41f),
                LandZone = new Vector3 (510.55f, 36.63f, 283.3f),
                GatheringType = 3,
                NodeSet = 16
            },
        } },
        // Botanist | 8 Nodes | #7
        { new Vector2(506, 682), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35151,
                Position = new Vector3 (559.91f, 55.79f, 672.96f),
                LandZone = new Vector3 (559.79f, 55.21f, 672.69f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35158,
                Position = new Vector3 (536.1f, 55.63f, 679.65f),
                LandZone = new Vector3 (536.37f, 54.9f, 679.87f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35154,
                Position = new Vector3 (520.14f, 56.14f, 694.22f),
                LandZone = new Vector3 (520.91f, 55.61f, 693.97f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35155,
                Position = new Vector3 (502.04f, 56.63f, 680.56f),
                LandZone = new Vector3 (502.08f, 55.68f, 680.68f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35157,
                Position = new Vector3 (481.21f, 56.12f, 660.01f),
                LandZone = new Vector3 (481.17f, 55.23f, 660.35f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35156,
                Position = new Vector3 (489.9f, 56.31f, 671.44f),
                LandZone = new Vector3 (490.2f, 55.49f, 671.69f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35152,
                Position = new Vector3 (464.35f, 55.87f, 661.57f),
                LandZone = new Vector3 (464.23f, 55.16f, 661.27f),
                GatheringType = 3,
                NodeSet = 17
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35153,
                Position = new Vector3 (452.62f, 56.02f, 663.78f),
                LandZone = new Vector3 (452.79f, 55.22f, 663.5f),
                GatheringType = 3,
                NodeSet = 17
            },
        } },
        // Botanist | x Nodes | #8
        { new Vector2(609, 478), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35183,
                Position = new Vector3 (522.22f, 53.2f, 514.22f),
                LandZone = new Vector3 (522.16f, 52.29f, 515.22f),
                GatheringType = 3,
                NodeSet = 18
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35184,
                Position = new Vector3 (529.68f, 54.04f, 488.3f),
                LandZone = new Vector3 (529.51f, 53.32f, 488.7f),
                GatheringType = 3,
                NodeSet = 18
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35185,
                Position = new Vector3 (579.7f, 54.15f, 412.27f),
                LandZone = new Vector3 (579.61f, 53.31f, 412.66f),
                GatheringType = 3,
                NodeSet = 18
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35186,
                Position = new Vector3 (592.29f, 54.46f, 429.05f),
                LandZone = new Vector3 (592.12f, 53.71f, 428.87f),
                GatheringType = 3,
                NodeSet = 18
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35187,
                Position = new Vector3 (671.4f, 64.6f, 498.43f),
                LandZone = new Vector3 (670.72f, 63.73f, 498.07f),
                GatheringType = 3,
                NodeSet = 18
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35188,
                Position = new Vector3 (651.13f, 57.95f, 522.03f),
                LandZone = new Vector3 (651.07f, 57.13f, 521.38f),
                GatheringType = 3,
                NodeSet = 18
            },
        } },
        // Botanist | x Nodes | #9
        { new Vector2(527, 630), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35194,
                Position = new Vector3 (595.76f, 58.28f, 648.99f),
                LandZone = new Vector3 (595.16f, 57.34f, 650.08f),
                GatheringType = 3,
                NodeSet = 19
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35193,
                Position = new Vector3 (602.08f, 59.59f, 659.02f),
                LandZone = new Vector3 (601.25f, 58.52f, 658.62f),
                GatheringType = 3,
                NodeSet = 19
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35192,
                Position = new Vector3 (484.43f, 56.59f, 676.53f),
                LandZone = new Vector3 (484.55f, 55.62f, 676.34f),
                GatheringType = 3,
                NodeSet = 19
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35191,
                Position = new Vector3 (463.9f, 55.91f, 671.28f),
                LandZone = new Vector3 (464.47f, 55.17f, 670.95f),
                GatheringType = 3,
                NodeSet = 19
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35190,
                Position = new Vector3 (468.77f, 55.87f, 591.9f),
                LandZone = new Vector3 (469.39f, 55.08f, 592.1f),
                GatheringType = 3,
                NodeSet = 19
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35189,
                Position = new Vector3 (492.08f, 54.21f, 574.35f),
                LandZone = new Vector3 (491.81f, 53.34f, 574.8f),
                GatheringType = 3,
                NodeSet = 19
            },
        } },
        // Botanist | x Nodes | #10
        { new Vector2(-706, 564), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35196,
                Position = new Vector3 (-684.35f, 58.86f, 503.32f),
                LandZone = new Vector3 (-684.05f, 58.24f, 503.64f),
                GatheringType = 3,
                NodeSet = 20
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35195,
                Position = new Vector3 (-701.75f, 62.75f, 512.91f),
                LandZone = new Vector3 (-701.3f, 62.17f, 513.41f),
                GatheringType = 3,
                NodeSet = 20
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35200,
                Position = new Vector3 (-772.7f, 69.54f, 593.97f),
                LandZone = new Vector3 (-773.43f, 68.96f, 593.82f),
                GatheringType = 3,
                NodeSet = 20
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35199,
                Position = new Vector3 (-770.47f, 70.37f, 608.22f),
                LandZone = new Vector3 (-769.76f, 69.6f, 608.24f),
                GatheringType = 3,
                NodeSet = 20
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35198,
                Position = new Vector3 (-651.49f, 56.65f, 620.18f),
                LandZone = new Vector3 (-651.85f, 56.27f, 619.56f),
                GatheringType = 3,
                NodeSet = 20
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35197,
                Position = new Vector3 (-649.1f, 57.32f, 604.2f),
                LandZone = new Vector3 (-649.86f, 57.03f, 604.38f),
                GatheringType = 3,
                NodeSet = 20
            },
        } },

        // Criticals
        // Botanist - Critial #1
        { new Vector2(566, -908), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 0,
                Position = new Vector3 (),
                LandZone = new Vector3 (),
                GatheringType = 2,
                NodeSet = 24
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 0,
                Position = new Vector3 (),
                LandZone = new Vector3 (),
                GatheringType = 2,
                NodeSet = 24
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 0,
                Position = new Vector3 (),
                LandZone = new Vector3 (),
                GatheringType = 2,
                NodeSet = 24
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 0,
                Position = new Vector3 (),
                LandZone = new Vector3 (),
                GatheringType = 2,
                NodeSet = 24
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 0,
                Position = new Vector3 (),
                LandZone = new Vector3 (),
                GatheringType = 2,
                NodeSet = 24
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 0,
                Position = new Vector3 (),
                LandZone = new Vector3 (),
                GatheringType = 2,
                NodeSet = 24
            },
        } },
        // Botanist - Critial #2
        { new Vector2(188, -201), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35225,
                Position = new Vector3(178.5146f, 28.33517f, -234.279f),
                LandZone = new Vector3(178.5146f, 28.33517f, -234.279f),
                GatheringType = 3,
                NodeSet = 25
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35226,
                Position = new Vector3(197.6805f, 31.65541f, -243.8829f),
                LandZone = new Vector3(197.6805f, 31.65541f, -243.8829f),
                GatheringType = 3,
                NodeSet = 25
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35230,
                Position = new Vector3(144.964f, 22.65566f, -198.4546f),
                LandZone = new Vector3(144.964f, 22.65566f, -198.4546f),
                GatheringType = 3,
                NodeSet = 25
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35229,
                Position = new Vector3(167.5034f, 24.00558f, -186.3173f),
                LandZone = new Vector3(167.5034f, 24.00558f, -186.3173f),
                GatheringType = 3,
                NodeSet = 25
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35228,
                Position = new Vector3(230.1051f, 26.79718f, -167.2313f),
                LandZone = new Vector3(230.1051f, 26.79718f, -167.2313f),
                GatheringType = 3,
                NodeSet = 25
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35227,
                Position = new Vector3(234.4044f, 27.3815f, -175.0399f),
                LandZone = new Vector3(234.4044f, 27.3815f, -175.0399f),
                GatheringType = 3,
                NodeSet = 25
            },
        } },
        // Botanist - Critical #3
        { new Vector2(748, 101), new List<GathNodeInfo>() {
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35213,
                Position = new Vector3(778.8542f, 58.51204f, 90.23239f),
                LandZone = new Vector3(778.8542f, 58.51204f, 90.23239f),
                GatheringType = 3,
                NodeSet = 26
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35214,
                Position = new Vector3(785.6716f, 59.13748f, 69.10181f),
                LandZone = new Vector3(785.6716f, 59.13748f, 69.10181f),
                GatheringType = 3,
                NodeSet = 26
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35215,
                Position = new Vector3(724.3608f, 56.97183f, 149.7798f),
                LandZone = new Vector3(724.3608f, 56.97183f, 149.7798f),
                GatheringType = 3,
                NodeSet = 26
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35216,
                Position = new Vector3(733.382f, 57.84801f, 152.2305f),
                LandZone = new Vector3(733.382f, 57.84801f, 152.2305f),
                GatheringType = 3,
                NodeSet = 26
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35217,
                Position = new Vector3(710.4752f, 53.412f, 49.43619f),
                LandZone = new Vector3(710.4752f, 53.412f, 49.43619f),
                GatheringType = 3,
                NodeSet = 26
            },
            new GathNodeInfo
            {
                ZoneId = 1237,
                NodeId = 35218,
                Position = new Vector3(700.0176f, 52.94641f, 56.65482f),
                LandZone = new Vector3(700.0176f, 52.94641f, 56.65482f),
                GatheringType = 3,
                NodeSet = 26
            },
        } },
    };

    public static Dictionary<uint, List<string>> FishingPreset = new()
    {
        #region D Rank

        [451] = new List<string>
        {
            "AH4_H4sIAAAAAAAACu1WyW7bMBD9lYBnCdBCLdTNcZ00gJMGdYoegh5oaSQRlkWHpNK4hf+9oCTalmNnKYKeepNnefNmNHrj32jUKD6mUslxXqDkN5rUdF7BqKpQokQDFtLOKath58yM6ypDiRcTC90KxgVTa5S4FrqSk6e0ajLIdmYdv+mwrjlPSw3WPnj6qcUJYwtdru5KAbLkVYYS13EGyC9DtxgkGmQ4r5IZl83yRGPYdfArjAwIrypIlekEu467H+a9zoKLjNHKAIQuHgDgPuyCyXKyBrlXKDhgGAQDhqGZOV3ArGS5Oqes5akN0hhmiqYLiZKgn2IYP8fdRyU96i1VDOoU9viEh3nhcGKeSRXsF4yp6jbBVD3M9g7m7ffZdyWtGF3IC/rIhQYYGEw7vjW0f4WUP4JAiauHZGriQQUzsHNWXNJl29moLioQ0qDqt5mhxI8c/IzuACrebCw0eVKC9l+WHvUdn/2kq6taNUwxXl9SVpsB2K6Fpo2Aa5CSFoAShCx005JAN7wGZHUI6xWgRE/iCN6US/XXeLcCJBxniGx0wt9VbP07PrMVpErQatwIAbX6oC4PUD+s16Nsn3V8tHob1S3ITPGV/kBZXcwUrFol3HHvl2gkPobyPlzL4VvNHhrQuMijFKckz20SedTGuZPacRhHNuRh7uRB5DmZhzYWmjKpvuS6hkTJfbeeuoHt1xwQHJ/meM15fXZOq0pj3XCxpNVnzhc62+jCd6CLnbhqrwSl6RuZ7U1dj9iNtLCY5JkSvC7ek+74e+lTKKDOqFi/G+ETb+bVlvsgwgvJNmDH72TIgMORqDvBVqcqRYHnb0NO1RoEvVCtj9MrOsoViDFtilJN2VIfA7dzHO5ue/Yb0V0b/bAnq50ABuT5fXzh1OkbbVTDbMpXeGiYgGymqGr0BdJ/Ag7X521b8uZl+P/O/+k7/7ZTpijNPQ+TwJ7HfmTj1PHtuesT2/VDgsPYcVLio80PI039H8X7raFTJ/2708Jeie5x4P44mzZ1Wp5NliAKqNP1UBOjlEQ0ILk9j4LYxlkENnGJZ4Mf+kGGU+JiH23+AB7XWwwNCwAA",
        },
        [452] = new List<string>
        {
            "AH4_H4sIAAAAAAAACs1V207jMBD9FTTPiZSL41zeSgUIqbBoyz4hHtxk0likcbAdWBb131fORU162UqIh32zxjPnnDkZTz5h1mgxZ0qreb6G5BOuKrYqcVaWkGjZoAXmcsEr3F1mw9VtBokXxRY8SC4k1x+QuBbcqqvfadlkmO3CJn/bYd0JkRYGrD145tTi0MiCm/qxkKgKUWaQuI4zQf43dIsRh5MK56yYedFsBgXEdcgZCUOVKEtM9ajQHad552mFzDgrT1jqepROTCV92TVXxdUHqhFxsKc4CCaK6WA6e8FlwXN9yXir2wTUEFhqlr4oSILeRhod4o5R4x71gWmOVYojPXS/jk4d9IZSyf/gnOluFI7NFY0OwLy9z+H3YI8FKzl7UdfsTUiDNwkM3fnWNP4TU/GGEhLXeHZCApkQDnZe8vUN27R9z6p1iVINJObbZ5D4oUMO1E+gou3WgqvfWrL+4ZkP8SiW76y+rXTDNRfVDePV4K3tWrBoJN6hUmyNkABYcN+KgHtRIVgdwkeNkBhjjuAthNJfxnuQqPC4QrDhxH3H2N7v9CxrTLVk5byREiv9TV3uoX5br0fVHnR8lL3N6gZkqUVtni+v1kuNdbsod9r7IZrJ75E8hms1/Kr4a4MGF0iGK4puYIc0zGySpiubYb6yHYKYe+iEHiGwtWDBlf6RGw4FydPzEOi39y5gmoLk6RO6Q78IAhrT0w3cYcmqtBAlZ5M+zH41Rs1yjXLOmnWhF3xjFpbZ/RlWmqesNA/QEHUJs41oqkmaoY/3H58/3YuRYWpkzlJclqw+sYNIEAdnVlCwteC/+aPtBurLY7R8Z7WJzI2rraHjwerHyRy78C7t2ICPx47kruczagc0DW0S5pG9oqFnBzSKPAfznNIAts8DXS/xiQTe88WCyTVezF4bpnl6YV4a32ClpnPt5n6Irkts6ruBTbI8tlcrEtqxH3tx5IeIGYXtXyZ/np3wCAAA" },
        [453] = new List<string>
        {
            "AH4_H4sIAAAAAAAACtVXSW/bOhD+KwbPEqCFWm+On5MacPOCKkUOQQ+UOLaJyKJKUk3zAv/3glpsyUvsFMFDe5OHnG++Gc7mVzSuFJ8QqeRksUTxK5oWJM1hnOcoVqICA+nDOStgd0i7oxlFsRNGBroTjAumXlBsG2gmpz+zvKJAd2J9f9NgfeY8W2mw+sPRXzWOHxroprxfCZArnlMU25Y1QH4busaIgoGGdZbMZFWtOwbYtvAZCp0Wz3PI1ImIYNuy+1rOeRZcUEbyE3i24/uDGONW7ZrJ1fQFZM8Bb88Bzxs44HdvQJ4gWbGFuiKsdkMLZCdIFMmeJIq9Nqp+eIjbR41a1DuiGBQZ9Pj4+3r+MKBOpyrYfzAhqsmMY2nmhwdgzt7ruC3Y/YrkjDzJa/KDC403EHTeucZQ/gUy/gMEim0dsxMU8MBgF84rtrwh69rvcbHMQcjOiH57imI3sPAB+wFUuNkYaPpTCTKowy0B/S73PHkm5axQFVOMFzeEFV2oTdtA80rAZ5CSLAHFCBnotuaEbnkBqEV4KQHFOk5H8OZcqt/GuxMg4ThDZKIT543F+nzHJykhU4Lkk0oIKNQHebmH+mG+HmV74PFR6/Wtay4yqIvumZTdYzdJlChe6hJnxTJRUNa9dedQm2hj8TF+9OFqYl8L9r0CjYsyh0QQ0cx0sW2ZOLMdM6JOYC6iwPH9AFwvomhjoDmT6t+FtiFR/PhaW9MObPuBF+HgNMcJl2uuVqO7qiQa7paLNck/cf6kAbrm8gCk/q3lEtS2TBYkl9DVbXvYj2krarzHdqCbVoeZKMGLXtWdV7fcnvocllBQIl4+gFcN/A+v0vycpwNFx4+2ejtvTl65hPER5XvBynfyCjzH3WqeYja49H5urbqul/FCgZiQarlSc7bWs81uDvYLqd5qKtEMT/3RGwtNx/aiw23gjUmuV5Cur3U5+wW+V0wATRRRlR6oese5MJEvy9fLlN+drf9DUl6s+afk6jt0/94U7nX90MKZE7ipSQPLMnFKPTOMbM90A+JAGPkuJRHafOvafrvWP24FTed/fEXDERDo5fj0CEhJrkZTyAfDyn4rNDMKhWIZyXU8tJ3mwnjNq2JwrR5A+1uYO1yQQ22pEguSQZLrJr2dXN6Z5dPbGOiP+WuzWxN+ezlInkmpJRMdxjqC/XWhXRL0ZyPeXTuWqr20SjPLydwgM2nggYnTFJsktQITu2CFVpimXuTXadXgthQfsed+Gz2AVCCK0QNRIEazQuq1ivFiuK5QGi6w52KT+hCZ2EuxmaYBmE5G8ML1wXJCH21+AYyCxvf6DgAA" 
        },
        [454] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACu1WS0/jMBD+K8jnRMrDeTi30i0sUmERLdoD4uAmk8YijYvtsLCo/33lJG6bPnis0J72lszjm2/G4xm/okGt+JBKJYf5HCWvaFTRWQmDskSJEjVYSCvHrIKNMjOqiwwlXkwsdC0YF0y9oMS10IUcPadlnUG2EWv7VYt1yXlaaLDmw9NfDU4YW+h8OS0EyIKXGUpcx+khvw3dYJCo5+G8S2ZY1IsjiWHXwe8wMiC8LCFVJhPsOu62mfc+Cy4yRksDELq4B4A7szMmi9ELyK1AwQ7DIOgxDE3N6QNMCparU8oanlogjWCiaPogURJ0VQzjfdxtVNKhXlPFoEphi0+46xf2K+YZV8F+w5CqthMOtVUY74F5O+X3O7BpQUtGH+QZfeJC4/UEJjvf6stvIOVPIFDi6podoYB7AU05T9n8nC6avAfVvAQhTRB91hlK/MjBe+x7UPFqZaHRsxK0u3f6IKZ88osuLypVM8V4dU5ZZWpruxYa1wIuQUo6B5QgZKGrhgS64hUgq0V4WQJKdGEO4I25VH+Ndy1AwmGGyEZH9G3ERr/hM1lCqgQth7UQUKkvynIH9ctyPch2L+OD0RurtkEmii/19WXVfKJg2czJDfeuiQbiayhvwzUcbiv2WIPGRXkQBT7Gvu14xLMx8X07Dj1iO3lIIMtns8j30MpCYybVj1zHkCi5a9tTJ7C+6wHB0XGOQy4XXBUn1/WSargrLha0/M75gwYwg+Mn0Oa/vXxaK0HpDMw17ERtmtiN9OQxzhMleDX/jLvjb7mPYQ5VRsXLpxG+8XpWrrn3LLyQrA02/I6a9DgcsJoKtjwWKQo8f21yLFbP6I1onZ3u0kGuQAxpPS/UmC30tnBbxW77Nu+CWrTrSH9sDdp2BgZkf4G+sQv1EjeDw3TKDTzWTEA2UVTVekXpV8Ju+3ysSz7cDP/P/J+e+e1mOHng4yjPPTsjaWzjmGA7ztLMDpzI80nqkCykaHVvplP3krxbC9oBpf/bcdgNozsc4PuTwWNNFUtPzjjPpKrzXPYnY+BHDiF+aEMEvo0DcO0YgtiOnCAmkUvy0M/R6g+i/MMeMQsAAA==" 
        },
        [455] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACu1Xy27bOhD9FYNrERAlUg/vHN8kDeCkQZ2LLoIuKGpkE5FFl6LSpoH//YJ62JZjJXGRRYHenTCcOTwzPByOntGkMmrKS1NOswUaP6Pzgic5TPIcjY2uwEF2cSYL2C2m3dJVisZeFDvoVkulpXlCY+Kgq/L8p8irFNKd2fpvGqxrpcTSgtUfnv2qcYLIQZfru6WGcqnyFI2J6/aQX4euMeKwF+G+SWa6rFYDiVHi0jcYdSAqz0GYYRyyH+W9TUrpVPJ8AC8gtIdH26gLWS7Pn6DsCkqJyw74M9bjH3Qnwh9gvpSZOeOyzsIays4wN1w8lGjM2hoH0UvcfdS4Rb3lRkIhYI9PcBgX9OvpdaFa/oIpN41OjokuiF6AeQeH47dgd0ueS/5QXvBHpS1ez9Bl5zt9+xcQ6hE0GhNbswEKtLdhV84zubjkqzrvSbHIQZfdJvboUzT2Q5e+YN+DijYbB53/NJr3buWWgD2XOzX/wddXhamkkaq45LLoSo2Jg2aVhmsoS74ANEbIQTc1J3SjCkAtwtMa0NjW6QjeTJXmt/FuNZRwnCHCaGC92bFe3/GZr0EYzfNppTUU5oOyPED9sFyPsn2R8dHda68LpQXUl+4HX3eH3YhobtTaXnFZLOYG1nWn3SXUCm2iPyaPfbia2L+F/F6BxUWZF7LU8whmMVBMIWI4EjTFAtIojD3XS0mINg6aydJ8zuweJRrfP9e72QS2/YDFNBrmeK1UMTrjeW6xbpRe8fyTUg82uussX4E/7C6HXS2hV7nW1ORISWhbUxc8N1oVi1PCXX8vfAYLKFKun05G+EdVSb7l3vPwgnjrsOPXumxbQMbz8hB7P7JH7f3Bd1quh3iFzPO3Licy68Wezq0Nt/KfZAb0lFeLpZnJlX2qSLNweC/qkaXSzVtoP/a6fNOAWfzybX/lXbbzRdemOhV+ge+V1JDODTeVfR/tAHMozfekOCjM9wUPy/KE+KOifLfi/gytniDDv1PCe02cpSKJQSSYCpdjCiLGMYsz7AY8dTmP0wAI2nzrung7s99vDU0jv39G/Y4exP5wR5+UhouqHM34CgqjdLXqvUHktRJdpVAYKXhu62L3axwmK1UVPbf6XTkcrvz+3BvZnSqdcQHz3Dbr43M7i9kbIybbOOiP+Z3ZDQO/PQLYYGuZ2qrWBd0fCtpRwH425p3bMQXvqY0wLxTMSzAjhGAKxMOxIAF2oyCJKU1YIFK0cQ7VFLrhcAKzquB69AlWUqVV+b+S/g4lhUHC/SwLcej6gCnlEeZ+FmKR0CBL0pAFgV/3rQa3pXhPGfs2+gqwlsVidKtUPjoXKlcLq4PRvNKP8NQfcqnrsSRgMRYRCEwZcXESsgRTIkISZEnigUCb/wC50kcDPhEAAA==" 
        },
        [456] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACq2UTW+jMBCG/0o1Z5CAgAncaJRWldJutemeqh6mZghWXZy1TbfdVf77ynwoIU230qo3mPE8887rgT9QtFYt0FizqDaQ/4Flg4+SCikht7olD1xyJRraJ8sxdVVCHs0zD261UFrYN8hDD67M8pXLtqRyH3bndz3rWileO1j3ELmnjsPmHlxu72pNplayhDwMggn53+iOkaWTiuBTMYu6fR4VxGEQfyJhrFJSErcHheHhsejztkqXAuUHloYRYxNT46HsQph6+UbmoHFypDhJJorZaDo+0boWlT1H0el2ATMG1hb5k4E8GWxk8/fcQ2o2UG/RCmr4R6sRhwE7xrCpodFI0uI3LdD2mzGKOK6Ojq5jNlTf1SgFPpkLfFHaASaBcbqZN41/J65eSEMeOs9OrTabu404aDjaeS42l/jczV00G0najE3c3ZeQz9Igfqd+gprvdh4sX63G4cNzF3Gn1r9we9XYVlihmksUzeiHH3qwajVdkzG4IcgBPLjpRMCNagi8nvC2JcidMSd4K2Xsf/NuNRk6rRB8+CDfd+zyez3rLXGrUS5aramxXzTlEfXLZj2p9t3EJ7t3p/oFWVu1dZ+vaDZrS9vuR7nXPixRob9GcnGA6zT8aMTPlhwXwjSrEpZyH4Oo8uPkMfUx5qnPMH5MsOI8C0vYebASxn6rXA8D+f3DGBj+3vuAG6p/7xUMGu/jhD2cFcYobak8KySv6VlwlGfXaEkLlGaqCzEteRyjHwazxI+jlPwsq1Kf8SQllrIkiyvY/QW/guJLsAYAAA==" 
        },

        #endregion

        #region C Rank

        [457] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACq2US2/iMBCA/0o150SK805ugChCol20dE9VD8YZiEUas7bTLVvx31fOQxAeRVpxS8b2N9+MJ/mCQaXFiCqtRqs1pF8wLumywEFRQKplhRaYxRkv8bCYdUvTDFI3TiyYSy4k1ztIiQVTNf5kRZVhdgib/fuG9SQEyw2sfnDNU80JYwsm25dcospFkUFKHKdH/h5dM5Kod8K5KTPKq/crhfnE8W8YdRBRFMh0V4lPHHK8zb1tIWTGaXFFhLhh2Oux3x575Cof71AdJQ5OjIOgZxx2d0A3uMj5Sg8pr71NQHWBhaZsoyAN2q6G8Tn3mJq01DnVHEt2bVJ84oSnmLDfULcjSf4XR1Q3g9JJnJ52T67Da0+/5LTgdKMe6YeQBtALdNV5Vj/+E5n4QAkpMT27NOlhbCbiKGHXziFfT+h7XfegXBcoVZfE3H0GqRc5/pl9DxXv9xaMP7Wk7XdoLuJFLP7Q7bTUFddclBPKy64fNrFgVkl8QqXoGiEFsOC5loBnUSJYDWG3RUhNYy7wZkLp/+bNJSq8bAg2XFlvMtbrB5/FFpmWtBhVUmKp71TlCfVutV60Pav4YvZ6VzMgCy225vPl5XqhcVv/Nw/u7RAN5H2UB0e42uFXyX9XaLgQxRjHTuzZSeSi7TOP2XHmOraH0ZJFGfPCcAl7C2Zc6R8rk0NB+vrWBdqf+SFgimreG4PW8dUPoreHIV8/1Bv6CtkyCeiK2SGJqe07wcpOktCzmUPciPgkoiyD/T8ep8QgqgYAAA==" 
        },
        [458] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACq2U227iMBCGX6Wa60TKyTndUUQrJNpFC3tV9cIkU2I1tVnbYctWvPvKOYiEQ5FW3Dkznn+++TPJF4wqLcZUaTV+W0P6BRNOVyWOyhJSLSu0wCRnjOMhmXepaQ6pFycWzCUTkukdpK4FUzX5zMoqx/wQNvf3jdaTEFlhxOqDZ061Thhb8LhZFhJVIcocUtdxBsrfS9caSTSocK7CjIvq48JggesEV4g6EVGWmOluksB13P417zqFkDmj5QUQ1wvDgcdBW/bAVDHZoeo1JkfEhAyIw+4d0HdcFOxN31NWc5uA6gILTbN3BSlpXQ3jU92+atKqzqlmyDPs8YTHdeHQQa8rlewvjqluNqPrelztHfnvt9XLgpaMvqsHuhXSCAwC3Ti+NYz/xExsUULqGpPOrXYYmxXoNez8u2frR/pRDzri6xKl6pqYl51D6kdOcEI/kIr3ewsmn1rS9sMzzi/F4g/dTLmumGaCP1LGOz9s14JZJfEJlaJrhBTAgucaAp4FR7Aahd0GITXGnNGbCaX/W28uUeF5QrDhQr7pWOcPPIsNZlrSclxJiVzfaMoj1ZvNepb2ZOKz3etbzYIstNiY75Xx9ULjpv5RHtjbJRrJ2yCPenI1wy/OfldodAF9JCSMiU19n9hBkkR2EmFikzBfIU0CP6Ie7C2YMaV/vJkeCtKX1y7Q/r0PATNU89wQtIwvAYlf7yZ8y6TgH8g1Le+mXBmXmOBDJCcMPS93PJuQbGUHlEY2jXPPdpxo5QUkTlxCYP8POg4fC6sGAAA=" 
        },
        [459] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACs1VTU/jMBD9K2jOieSk+b6VbkFIhUUb9oQ4OMmksUjjrO3Asqj/feV8KElptxLisDdrPH7z5vl5/A7LRvEVlUqu8i1E77CuaFLisiwhUqJBA/TmhlU4bmbD1k0GkR2EBtwLxgVTbxBZBtzI9e+0bDLMxrDO33dYt5ynhQZrF7ZetTheYMB1/VAIlAUvM4gsQmbI/4ZuMUJ/doKcJbMqmt3AwLGIc4bCcIqXJaZqctCaptnny3KRMVqekNSyPW8mqtMfu2KyWL+hnBR2Dxi77oyxN4hOnzEuWK4uKWt564AcArGi6bOEyO1l9IKPuFPUsEe9p4phlZ6yhmMR7xDGmwtqD0iC/cEVVZ0zBhKHp+2D61j0px8KWjL6LK/oCxcaYBYYulsY8/gPTPkLCogsrdkxa3uBdsSk4CDnJdte013b97LalijkUETffQbRwifOB/YzqGC/N2D9WwnaPzx9EQ88fqX1TaUaphivrimrBj1My4BNI/AWpaRbhAjAgLuWBNzxCsHoEN5qhEgLcwRvw6X6NN69QInHGYIJJ/a7iu3+yCeuMVWClqtGCKzUF3V5gPplvR5l+6Hjo9XbrM4gseK1fr6s2sYK63ZQjtx7Ey3F11CewrUcflbsV4MaF7zcD+3ES00rcTLTQds3A8sh5iLzUzvMPeKlIewN2DCpvue6hoTo8WkI9NN7DOimIHp8h27RDybXt+3TDWyaioqLJaoCRckbOe9GT1kt1zJXKFa02RZqw3Z6bOkfIMNKsZSW+hnqcl3CcsebapJ2bBq54eGLXMyHZaALNyKnKcYlrcdmQvfMIHL3Bvw3/9poq0+bKX6ltY6stKqtoFN79abSyy48ph2z+cR8gZf5Qea4JvWt3HRCkphJRgJzERJCE+I6gUNg/zSU6yk+Om74dHHPS6p/iezim/4CZ7jWwidWkhIzTHLLdGxMTapXaOWpE9Iw8AmF/V+jCGXh7QgAAA==" 
        },
        [460] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACu1XTW/bOBD9KwbPJqAPkhJ9c71pNoCbDaosegh6GEsjmwgjuhSVbbbwfy8oWbHl2Elb5LDY5iaTM2/ejJ5mxt/ItHFmBrWrZ+WSTL6RswoWGqdak4mzDY6Jv5yrCneXRX91UZBJlMoxubLKWOUeyCQck4v67GuumwKL3bG333RYH4zJVx6sfYj8U4sj0jE5X1+vLNYrowsyCYNggPw8dIshk4FH8CKZ2aq5O5EYCwP2AqMexGiNueszYWEQ7ptFL7MwtlCgTxAJIyEGNWZbt/eqXp09YL0XmB8w5nzAWPTvAG4xW6nSvQPV8vYHdX+QOchvazLh26qK9CnuPqrcol6BU1jluMdHHPqJYQWj3tWqf3EGrlNGH/XQOzqof7z1vl6BVnBbv4d7Yz3A4KBPJx4Pzz9ibu7Rkknoi3RM2iL1EtgL2NfvnVqew12b6LRaarR1H8S/7IJM4iRgT9gPoNLNZkzOvjoL2w/PV/7aZP/A+qJyjXLKVOegqr4eNByTeWPxA9Y1LJFMCBmTy5YEuTQVknGH8LBGMvGFOYI3N7X7ZbwrizUeZ0goOXHfRWzvd3yyNebOgp411mLlXinLA9RXy/Uo2ycZH43eWnUCyZxZ++9VVcvM4bptlDvuWxFN7etQ3odrOfxdqS8NelySYpnmMQe6SGKkLCqBLgQDGvC4SDhgItOQbMZkrmr3V+lj1GRy08nTJ/BIUMrTDKdajzrXQ5qXxt6B/tOYWw/Ud4xPCO3v7iP0tzU6n0n/OW6POhwWJr7l9M6Zs6Za/ox7EO+5z3GJVQH24acR/jDNQj9yH1hEQj4a7PidNBlwOGJ1bdX6VKSER/GjyalYA6Nnom3tvFqnpUM7g2a5cnN158dE2F0cyrhdEBrbzSH/sNdwu17I5eEk9R335FD007xvIL1SPuKXRlksMgeu8bPJrwuH8vkxlfywGN7e+au+82fXsc2gSRW5DBKRxDSOYkZZkMdUChHSIkZRBgEgD1Oy+dx3qe1KefN40DWqm29kv2MxngTR6Z6VObCjK425GTSt8LnSXBRYOZWD9vXwcTqD6Z1pqoGZjy4PF4J4uJylPlJjS8gx077zHF9LueQvrEV8Myb/mS17N+R+ebR5Z38y81VtC7o/7LYjzj92xzuzY8rdU5ksUQIXQAUwTpksEwqFDGi5yMM8TlNMuB+FT1QUsdMJXKnq9s6YajRT+Uqr4k1Lv4eWEFEWCS9oFMYlZYUsqEQW0kLEvESJGLP8qJb46QTmTQV2lBk9TONNRf9bFYUJcihzpFESRpSxBCjkUFCRhsgli1IG/KiKxDNzTel7tKPMNXaJpnqT0u8hpUjiIhZxSgNIgTLJBQXJQophAEKkKcoC2hWqw91SvGEi+DzKTONWCLUbfUKtR2e50WbplTDKGnuPDwf/KDkuitRP0CT2oi0ZlZIlNAcUJQSx5GFJNt8BrZ+R62oUAAA=" 
        },
        [461] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACq2US2+jMBCA/0o0Z5B4xTxuaZRWkdJutXRPVQ8GhmCV2lnbdJut8t9X5qGEPBpp1RvM2N98MzZ8wqzRYk6VVvNyDcknLDjNapzVNSRaNmiBSa4Yx32yGFLLAhIvii14lExIpreQuBYs1eIjr5sCi33YrN91rHsh8srA2gfPPLUcEllwt3mqJKpK1AUkruOMyF+jW0YcjnY4V2XmVfN2obHAdYIrRgNE1DXmeugkcB33cJl33ULIgtH6gojrETKacdBvu2WqWmxRHRSeHhlPpyNjMpwBfcW0YqW+oaz1NgE1BFJN81cFybSfKolOuYfUuKc+Us2Q55duSuA65BhDxgP1BpJkf3FOdXdRBonj3d7Rcfj97qeK1oy+qlv6LqQBjAJDd741jv/EXLyjhMQ1Mzt300lkbsRBwWGcN2x9R9/avmd8XaNUQxFz9gUkfugEJ/YjVLTbWbD40JL236E5iCeR/qGbJdcN00zwO8r4MA/btWDVSLxHpegaIQGw4KGVgAfBEayOsN0gJGYwZ3grofR/8x4lKjxvCDZcyHcV2/zeJ91griWt542UyPU3dXlE/bZez9qedHy2eruquyCpFhvz+TK+TjVu2v/m3r2/RDP5PcqzA1zr8Iuz3w0aLkwzGvoxzW0/LEM7ICS0My/I7IhiFsVhSUL0YWfBiin9ozQ1FCTPL0Og/5nvA6ap7r0z6B2fA+K+TExyktImx8mSryUWDLlWY6G8LOMoCIntZhGxg9xxbFpkuY1+nGEcT73QK2H3DxIcHZC4BgAA" 
        },
        [462] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACq2UTW+jMBCG/0o1Z5AwMYZwS6M0qpR2q6V7qnpwYRKsEExt0223yn9fmQ8l5GMjrXKDGc8z7zse+IZJbeSUa6OnyxXE3zAr+VuBk6KA2KgaHbDJhShxl8z61H0GsR+NHXhSQiphviAmDtzr2Wda1Blmu7A9v21ZD1KmuYU1D759ajgscmBePecKdS6LDGLieQPyv9ENYxwOKryLYqZ5vekVUOLRCxL6KlkUmJozE6HEI/tV/mUVUmWCF2d4xGdsMGPald0Jnc++UO8ZCA4MBMHAAOvvgK8xycXS3HLR2LAB3QcSw9O1hjjopsqiY+4+ddxRn7gRWKbnNoUSjx1i2HC+fk9S4g9OuWkXpRdxWO0f3M6oq37OeSH4Wt/xD6ksYBDo3Y2cYfwnpvIDFcTEzuzUprPILshew36ct2I155vG96RcFah038TefQbxKPTokfoBKtpuHZh9GsW779BexLNMfvPqvjS1MEKWcy7Kfh4ucWBRK3xArfkKIQZw4LERAY+yRHBawleFENvBnOAtpDb/zXtSqPG0QnDhTL7t2OR3epIKU6N4Ma2VwtJcyeUB9WpeT6o9cnyye3OqXZDEyMp+vqJcJQar5r+5094t0URdR/JkD9do+FWK9xotF8IgWgaMMjfMiO/ScRq6Y865SxgJosyjy5RR2DqwENr8WNoeGuKX1z7Q/cx3AWuqfW8VdBpfKPNfbybvNTcivUn4pipQD5W8+YylXhi6PIoCl/qMuNE4oO4o8LI0DNnIi0LY/gXkXb+IsQYAAA==" 
        },
        [463] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACu1XTW/jNhD9KwbPIiBK1Ad187pJGsBJgzpFD0EOtDi0iciiS1KbTQP/94KyBFv+2ACLBMgueiOGnDfvDR/H8isaN05PuHV2IheoeEUXNZ9XMK4qVDjTQID85lTVsNsU/da1QEWUswDdGaWNci+oIAG6thffyqoRIHZhf36zxbrRulx6sHYR+VWLk+YBulrfLw3Ypa4EKkgYDpC/D91isGyQEb5JZrJsVmeEURLSNxj1ILqqoHS9EkpCsn8sepuFNkLx6gwREqXpoMe0S7tUdnnxAnavcHLAOEkGjNP+DvgTzJZKui9ctbx9wPaBmePlk0VF0nU1zY9x91FZh3rHnYK6hD0+6WFeOuxg1Kca9S9MuNs6o696mB0d9D/usu+XvFL8yV7yr9p4gEGglxMHw/ifUOqvYFBBfJNOWTvNvQX2Cvb9+6IWV3zVCh3XiwqM7Yv4yxaoiLOQHrEfQOWbTYAuvjnDu4fnO3+vZ898fV27Rjml6yuu6r4fmARo2hi4AWv5AlCBUIBuWxLoVteAgi3CyxpQ4RtzAm+qrfthvDsDFk4zRBid2d9WbPd3fGZrKJ3h1aQxBmr3TioPUN9N60m2R4pPVm9PbQ0yc3rt36uqFzMH63ZQ7rh3Jhqb96G8D9dy+KtW/zTgcVFGgRAAgrkI55hKInFOohBn85hRwkLOckCbAE2VdX9IX8Oi4uGxD3TTexfwolDx8Iq2i+7lJ1mcnBdwAxWvy6WuFB/o8APVN2osHZgJbxZLN1UrP6H87BdQO1Xyyj9AX2h7YLzSTT045suzw8cXDwdh7is1RvISZhVfn/sJSFjyxghKNgH6NL9oO0P9sI18so9MfFfbhu4bq7OTX27Du2OnDL5nuxTCTAoicVpmFNM8pZixWOB5JjJBogiSkqJNcGyj7LyAaVNzM/pN2bKxn8dHP6Nx+lvvzX9sJK8oPH8VE21X+lmbFfpAD5E0oTwWFOccKKakFDgvkxCTMqQsT1IuEnbSQ+l54r/r9RoE1vVoClxKn/hpnPT/RPrIiRTFkEJIEiy8h2gqGc5FlGE/iygTMqZhdtJN+XkBM11xM7qsuIHPZaVfdihR9v2hpMrRxPAaRpfVy0fOJhFKTuZCYhJGCfZ/ATFPc4Zj4GUEwGnIBdo89uU6hg80jR9Ht9q45TNYB6Ye/c0dmNF1bf0HpdL18OstBiqlKFMsEwgxlQIwJ/McMzHPGIupZCREm/8A7WmxAdYPAAA=" 
        },

        #endregion

        #region B Rank

        [464] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACt1Wy27bOhD9FYNrCdCDeu5c3zQx4KZBnSKLoAtKGtmEZdElqaS5gf+9oCTakiwnbpHFxd0Jw5nDM6PhmXlF00qyGRFSzPIVil/RVUmSAqZFgWLJKzCQOlzQEo6HmT6aZyh2wshAd5wyTuULim0DzcXVr7SoMsiOZuW/b7C+MJauFVj94aivGscPDXS9u19zEGtWZCi2LauH/DZ0jREFvQjrXTKzdbU9kxi2LfwOIw3CigJSqTPBtmV33Zz3WTCeUVJoAN/GPQDcun2mYn31AqJzkTdg6Hk9hr6uOdnAck1z+YnQmqcyCG1YSpJuBIq9top+eIrbRY1a1DsiKZQpdPj4wzi/XzFHh3L6L8yIbDpB3zqMdgb1dtvo+zUpKNmIz+SJcQXQM+h0XKNv/wYpewKOYlsVSd+Jezfogn2iq2uyrTOblqsCuNCo6m9mKHYDC5/Q7UGF+72Brn5JTtqXpUp9z5bPZDcvZUUlZeU1oaUugGkbaFFx+AJCkBWgGCED3dYk0C0rARkNwssOUKwqMYK3YEL+Nd4dBwHjDJGJzpw3N9bnRz7LHaSSk2JWcQ6l/KAsB6gfluso25OMR2+vvZoGWUq2Uw+UlqulhF2thEfubRNN+cdQ7sLVHL6X9GcFChelmYtt18ImJC42sY8TMwkc14xcy4XAhQwsQHsDLaiQX3N1h0DxY9OeKoEDwSg6z3BaFJMmdEjzlvEtKW4Y2yggLREPQDZHnVWnAqTKRCtua2pwsB0ojdHBS8lZWT+h1uug1jkpBBgXo1puB3UBKygzwl8+Cvi7gH9Y1fprx8ai0/9aFi8PayinqaRPMM+glDRVwj+C6viqBk38uxU4G3lJliPB95zujrT7DoHnuAeXE2ZjTmMk+n7q9UxzCXxGqtVaLuhWzSm7ORg+q3ojqXgzCNVHR/FHBrkbeNFwnr25GqhtQuubbuRv8LOiHLKlJLJSs1KtK8Pu/qMmvrgpe479frq8YS7rjP9FC+h/jv/wn3c0NHISL0/DyMz8MDUxxtgkBLDpYSeyfBwExM7R/ocW0XalfTwYGh19fEVdQcVegN3zknrHqdgSSdNJHdTVVfut8hxURNVE3dU4TLesKjtuYxuuFw1XGLe/P4bq4ornJIVlodRPZ3LyoIarmrc30H9m0z/O4b+evstnslOWmapqXdDuPG6nsPpszEe3se7tdJrrZ67jhblpexCZOHECkwD2TD/KLAjy1A5TUndag9tSfMQ+/jG5YUXBnic3hCeMTx6IBD6Zl0ItKpSV/a3AsSwnCVzLtN0wMXEKmUkch5hOgnOX+CTKiY/2vwEabWzoDw4AAA=="
        },
        [465] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACs1VTW/jIBD9K9Wcbck4GH/c0iitKqXdat09VT1gg2NUx2QB92Or/PcV/pDjNNlIVQ97Q8Pw5r3hMXzAvDFyQbXRi2INyQcsa5pVfF5VkBjVcAfs5krUfNxkw9YNg8SPYgfulZBKmHdIkAM3evmWVw3jbAzb/F2HdStlXlqwduHbVYtDIgeutw+l4rqUFYMEed4E+d/QLUYcTk54Z8ksymZzQhhGHj7DaACRVcVzMyjByEP7af55FlIxQasTRJBPyKTHuD92JXS5fOd6r3BwwDgIJozJcAf0maelKMwlFS1vG9BDIDU0f9aQBH1XSfQZdx817lHvqRG8zk85BSOPHMKQaUP9AUmJP3xBTWeUgcThaf/gOmb96YeSVoI+6yv6IpUFmAQGdTNnGv/Jc/nCFSTI9uyY00lkHbFXcGjnpVhf002re16vK670UMTePYNkFnr4E/sJVLTbObB8M4r279BexINMX+n2pjaNMELW11TUQz9c5MCqUfyWa03XHBIAB+5aEnAnaw5Oh/C+5ZDYxhzBW0ltvox3r7jmxxmCCyf2u4rt/sgn3fLcKFotGqV4bb5J5QHqt2k9yvaT4qPV26zOIKmRW/t8Rb1ODd+2c3Pk3ptorr6H8j5cy+FXLX433OIC8mY+Z4S5fuQhF/skdLM88N0YFZhhz2cxjmHnwEpo86OwNTQkj09DoB/mY8CKguTxA7pFP5iCEEenBayamqqLheQVzWltyokYO2Rtt+aF4WpBm3VpVmJjp5b9DxivjchpZV+hrdYlzDeyqffSjg2jID58kLPprIxs4UYVNOdpRbejljg4M4eCnQP/zS83uurLXkpf6dZGFrarbUP33dV7yi678Jh2zOV73mMz7OMgz1xWkJmLM4TcbFZkLs4y5DFaEBJi2D0N5XqKj5gETxdtSL1wdnElJdOmKQo9dXYWkZCyyHfDCMUujnjkRmEYuD4lGAe0QGGMYPcXdH5qeAEJAAA="
        },
        [466] = new List<string> 
        { 
            "AH4_H4sIAAAAAAAACu1XbW+jOBD+K5E/YwmwDYZv2Vz6IqXdatPTfahWpwFMgkpw1ph2c1X++8kQGkhCX1bVaU/ql4ra42eeGT+emTyhcaXlBEpdTtIFCp/QtIAoF+M8R6FWlbCQ2ZxlhdhvJu3WZYJClwcWulGZVJneoNCx0GU5/RnnVSKS/bKx3zZYV1LGSwNWf7jmq8bxuIXO17dLJcqlzBMUOrbdQ34ZusYI/N4J+1Uyk2W1GgiMOjZ9hVELIvNcxLqNhDq20zVzX2chVZJBPkDEc2gPj+5OnWXlcroRZccvOyDMWI+w114B3Iv5Mkv1F8hq2mahbBfmGuL7EoVsl1SPH+N2UYMd6g3oTBSx6PDxDs95/QS67VGV/SMmoBthtF4PT7sH6Se707dLyDO4L8/gQSoD0FtowyFWf/2biOWDUCh0TJJOKdvjRgEdh23+vmSLc1jVgY6LRS5U2Toxd52gkPg2PWLfg+LbrYWmP7WC3bszmb+V80dYXxa6ynQmi3PIijYf2LHQrFLiSpQlLAQKEbLQdU0CXctCIKtB2KwFCk1iTuDNZKl/Ge9GiVKcZogwGthvPNb7ez7ztYi1gnxSKSUK/UFRHqB+WKwn2R5FfNJ7bdUIZK7l2rzXrFjMtVjXdXLPfSeisfoYyl24msOfRfajEgYXUW67NnAX8zj2MA1SgrkXA/b9gDDiAEsIQ1sLzbJSf02NjxKFd408TQDPj5sFjA5znMhylcWjW1BQ6CoHA3kt1QryCynvDUhbLf4SUP/fPECzWwptomif4m6pCZU6vik37eG5VrJYvOe4TTrHZ2IhigTU5t0If8gqyp+59yxcL3g22PMbNOlxOGF1q7L1kCefueTZZMhXz+gFbzs7o9RxqoWaQLVY6lm2Mi3CaTYOJVzPBpVqepD56BTbpg6y4LiJvtAPTSNvi0erlG/iR5Upkcw16Mr0JTMpHMrnbSp5sxg+7/w/vfNOgSIOSVLq2jiKPR9TKgDzGDiGxHapG0UxJz7afm8r1G6avHteaIrU3RPqVyufucPV6usa8tFU5L2q6ryUmMtEFDqLITfZMF4ag/FKVkXPrK6Uh6MA6Y9l3HiqVAqxmOem7uxL7CsTENta6LeZp03NbAbKpm7u+1s3IjZ8DWeG6iNooUZjpZdK5pUSqIM8MdlthPkI66blla27bgdEIYJCFn/fUc/DzvfRhcxz+Ti6ABVJNZrGMpcLc3ujeaUexAZ1ATtOTqi/o1SfuFHkEYIpd1NMqR/jgFHzBxiNGE3BDdDWOlYiGU7BrCpAjW4yBaqKq089furx7XqkNiQcbIGJAwRTkdoYksDHjLLYSYnHRcJP6ZEEwym4krJQMr4fTaBINr+PHk/9Xv+fyfNYju8e+udHOjwQnRn+f0FJxCNe5NkBjuI0wdQlAeYkcjB3me8woB4Ar3twg7ujWIvbfYu4O64S2/XBdwnmjHJMaepjHrs29hhlqcco576Dtv8C0hItuqcSAAA=",

            "AH4_H4sIAAAAAAAACu1X227jNhD9FYPPIiCKFHV583pzA5w0WKfoQ7AoRhRlC5FFL0Ul6wb+94KSlUi+ZbvIQ7btm0MOzzkzPJphntG4NmoClakm2RzFz+ishKSQ46JAsdG1dJDdnOalfN1Mu62rFMVeGDnoVudK52aNYuKgq+rsuyjqVKavyzZ+02JdKyUWFqz54dlfDQ4PHXSxultoWS1UkaKYuO4A+TR0gxEFgxPum2Imi3p5JDFGXPaGog5EFYUUpsuEEZf0w7y3VSid5lAcEUI8zgc1Zttj53m1OFvLqkfs7yj2/YFi3t0BPMjZIs/MJ8gb3Xah6hZmBsRDhWJ/W1Ue7uP2UaMt6i2YXJZC9vTw3XN8WEGvO6rzv+QETOuMjnX3tLdTf7o9fbeAIoeH6hwelbYAg4UuHeoM179IoR6lRjGxRTpkbR5aC/QIu/p9yucXsGwSHZfzQuqqI7GXnaKYBi7bUz+ACjcbB519Nxq2H56t/J2aPcHqqjR1bnJVXkBedvXAxEHTWstrWVUwlyhGyEE3jQh0o0qJnBZhvZIotoU5gDdVlflpvFstK3lYIcLoyH7L2Oy/6pmtpDAaikmttSzNO2W5g/puuR5Uu5fxQfYmqjXIzKiV/V7zcj4zctU0ylftWxON9ftI7sM1Gn4v82+1tLjIlz5JIeKY+DTBLAwpBgYcC54BZ5QGkcfQxkHTvDK/ZZajQvF9a0+bwMvH7Ue+f1zjubX9ExipR2NtFloVtZYW90bpJRSXSj1YpK5l/CGh+duu2zSatBgJbGvpYmZGq3J+IMqlvaipnMsyBb0+FvhZ1UlxmNDj0UvAEbZ+yHGqNupO56tjTIHv0ZeQY1yDoBNs2zjrsXFmpJ5APV+Yab60zZ20G7vma8Z6rdvpYX/02mTbwfxof/6dGGV2BneffXe9X+S3OtcynRkwtZ0odsj/f+f/pjvvtRY3zXgUpB7OPBFixlyCk8AFDD7lPAiiLAQXbb52vWX7ELx/WWjby/0zGvaZwD/RC6d1CXo0gUQtExg0RXKqOlepLE0uoLAlsVRtwHip6nIQ1jS63UlOh6+q0DLVOgMhZwWs+h3yjQeMv3HQh3kP28nRvgfbJ9DreOpnxI7fxURVy1yM7kBDaeoCUA90YgvbGvMJVu2wqjqm/uxCMYJSlX/eM86x93V0qYpCPY0uQSdKj86EKtTcXtxoVutHuUZ9wB7JAff3nEqAkkT6GSYkA8ySlGIQjOCIUxJCwAM3EGjj7DmRRsezv1aq1Eo8jCZQpuuPY8VD/9r8Ys7cd+I/fh7t+3DHdPjnnCRp5mYik1hkGcPMkwQnNIuw9MMwTNPUozw95CTmnnbSPC+Kj+OhX9E0/6F25vlpyiHj2IMswowmAU6YR3EiQ+FCRChx02bwtrjb/BpJ5Eck9agEY77rMolT6XPMLEtIZIhdKkTmUREJ6qLN35MG6TZXEgAA",
        },
        [467] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1WS2/jOAz+K4HOFmBbsvy4pdm2WyDTLcZd7KGYg2zTiVDHykhyZ7pF/vtAfiRxHs2g6GEPe7Mp8uNHiiL5hqaNkTOujZ6VC5S8oeuaZxVMqwolRjXgIHs4FzXsDovh6K5AiR/FDnpQQiphXlHiOehOX//Mq6aAYie2+psO64uU+dKCtR++/WpxWOSg2/XjUoFeyqpAiee6I+T3oVuMOBxZuBfJzJbN6kxg1HPpBUYDiKwqyM15HG/fyr9MSqpC8OoMnuczNko57c1uhF5ev4IeMko9NzgIIAhGAbDhSvgzpEtRmisu2jCsQA+C1PD8WaMk6JPMomPcfdS4R33gRkCdwx4fdmjHxgn1B1Ml/oUZN12hnKo6Fh2B+Qe3Q3qwxyWvBH/WN/xFKos3EgzREWcs/wq5fAGFEs/m7AwFOnI4pPNKLG75qo17Wi8qUHpwYu++QAkJXXrEfgQVbTYOuv5pFO+fpb2IR5n+4Ou72jTCCFnfclEPucWeg+aNgi+gNV8AShBy0H1LAt3LGpDTIbyuASU2MSfw5lKbD+M9KNBwmiHC6Mx557E93/FJ15AbxatZoxTU5pOiPED9tFhPsj2K+KT3VqsrkNTItX2+ol6kBtZtG91x74toqj6H8j5cy+HvWnxvwOIiHngReCXHLMwYpjmNcBQCYMiY6xJSlHlYoo2D5kKbv0rrQ6PkqStPG8D2rQdxQM5znDc1V5M5Vy/cot1LteLVn1I+W/uhb/wDvP23cg1m+wRLXmkYnmR/aOMaHmcv6oKnXmj70YCZGiXrxSegumQPdQ4LqAuuXnfd6jcR/pBNVh1G2mn4LN4qHNE+VhlxOKH1qMT6nKcw8MlW5ZyvkdI73no9W9LT0oCa8WaxNHOxsqPF6w4Oa73dMRrVzS77sdeVT7ReEgbx8Wx+Z67a/WBoOkOZfYXvjVBQpIabxo43u4Ccqb3fq6XLtfF/CXyoBD5653uNzQ3cMCAex6yMPUx57uMoCnzMoiCMY0o8QgnafBs6W7+kPm0FXXN7ekPjLhcG0aUudyXypVCjjuy9l5y7Amojcl7ZjFhPncJ0JZt6pNZ22cM1gow3vMh6alTJc0gr24p27fnC9hRsHPSfWdV3s/DDEzD9wddWMrNpbDO4PxNRgqaZllVjYGKHtVhB7aPOqtPb2Z2q3r1KIyFEYckD7GZZjCkJQxwVEOKMMpIx5tI4ittK63B7zk+Uhd8mRxwmeHIjZTEe0uBHPKckx2ER+JhS5uIs9iJc+iXjBckC6jO0+QUMFBbR2w0AAA=="
        },
        [468] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1WS3OjOBD+Ky6doQqQEI+bx5vJpsqTTQVv7SE1BwGNUQUjjxCTyab836fEwwZi4p1UDnPYG7S6v/661eruF7SslVixSlWrbIvCF3RVsriAZVGgUMkaDKQP17yE02HaH92kKHT8wEB3kgvJ1TMKbQPdVFc/kqJOIT2Jtf6hxfoiRJJrsObD0V8NDvUNdL3f5BKqXBQpCm3LGiG/Dd1gBN7IwrpIZpXXu54BsS1ygUJvJYoCEjWTEWJb9tDKucxCyJSzYgbPdigd5Zh0Zp95lV89QzUIwJ0E4LqjAGh/B+wRopxn6hPjTRhaUPWCSLHksUKh22WV+q9xh6hBh3rHFIcymasUYlt0CkPH+XV6JMn/hRVTbaH0JKbWzuR2cGe9yVnB2WP1mX0XUgOMBH102BjL7yER30Gi0NY5O1fp1NcFMnDYp/MT316zXRP3stwWIKveib77FIXYs8gr9iMo/3Aw0NUPJVn3DvVFbET0xPY3paq54qK8Zrzs82HaBlrXEr5AVbEtoBAhA902JNCtKAEZLcLzHlCoE3MGby0q9W68OwkVnGeITDRz3npszk98oj0kSrJiVUsJpfqgKCeoHxbrWbavIj7rvdFqCyRSYq+fLy+3kYJ90zdP3LsiWsqPoTyEazj8XfJvNWhclDHHoh74Jk5syySZ5ZkBJYlJU8tyqG25PlB0MNCaV+qvTPuoUPjQlqcO4EgwCOYZLoti0ZpOad4KuWPFn0I8aqC+gfwDrPnX8grU8S1mrKigf5vdoQ6wf6WdqIUntqcbU48ZKSnKwYS7bG7hgfkatlCmTD7/MsIfoo6LaUithkODo8KJ36zKiMMZrY3k+zlPnuvgo8qcr5HSG946PV3Ey0yBXLF6m6s13+lhYrcH0+pu1ohattNKfwz68Jlmiz03eD2N35ikegXo20xfT/fwreYS0kgxVeuBpneMmSK7UDT/uTb+L4F3lcB773zQynwnpnGQOaZve5ZJsOOaAcXYhNj1M4sw7MUOOnzte1m3hz4cBW07e3hBw75GXI/i+c52z54W67pkcrERtW5lelEeNjn7rSTdpFAqnrBCZ0Z7bBWWO1GXA7Vzu5QbTPcJPF71fO24lhlLICp0h+rjCdwLa5R7MNBvs6SfhuK7R2H0xPZastJZbRI6HI7dSNSfrfikdq6GB/XmsQR71I9NFnvEJJi5pp84mZmyALAVU+aQuKm3Frej+ECo/3WxBJWDFEkOO33/i5UEppqnMXIBLmASp5mZ+jQwCUupGdgxNl1mUY/Y2LcdFx1+AhFfo4DFDQAA"
        },
        [469] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XTW/bOBD9KwbP5kIfpETp5njTbAA3G9RZ9BAUi7E0tIXIoktRadPA/72gZMWSY8dJkF1ssblJ5MybN8PHIXlPRpVRYyhNOZZzEt+T0wJmOY7ynMRGVzgkdnKSFbidTNup85TEnoiG5FJnSmfmjsTukJyXp9+TvEox3Q5b+3WD9VGpZGHB6g/PftU4gRiSs9XVQmO5UHlKYtdxeshPQ9cYUdjzcI6SGS+q5YHEmOuwI4xaEJXnmJg2E+Y6btfMO85C6TSD/AAR1wuCXo3Zxu1DVi5O77DsBOY7jDnvMQ7aNYAbnC4yaU4gq3nbgbIdmBpIbkoS801VA/EYt4sabVAvwWRYJNjhE+z6Bf0Keq2rzn7gGEyjjDbqrre3U39/4321gDyDm/ID3CptAXoDbTr+sD/+CRN1i5rEri3SPmkHwkqgE7Ct30k2P4NlneiomOeoyzaIXeyUxH7osEfse1BivR6S0+9Gw2bj2cpfqek3WJ0XpspMpoozyIq2HtQdkkml8SOWJcyRxIQMyUVNglyoAsmwQbhbIYltYfbgTVRpXo13qbHE/QwJJQfmm4j1/JbPdIWJ0ZCPK62xMG+U5Q7qm+W6l+2jjPdGr60agUyNWtn9mhXzqcFV3Si33DciGum3odyFqzn8VWRfK7S4xAsRZoIhjZiMKAsiSWehz2iaRoILxw8d3yfrIZlkpflT2hglia8bedoEHjY3j7h3mONYlUuVg74FC3ah9BLyP5S6se5tn/iMUP83W8/Olmgs/3YTboaaJJkb2kbTOk+NVsX8Je6O33Gf4ByLFPTdixF+V9Usf+Des/CC6MFgy++gSY/DHqsrna0ORQq55z+YHIrVM3oi2sbOanQkDeoxVPOFmWRLezi4zcSueOtrQaWb08d+dNps0wF59Pj8fOIotGd42zZapXzCr1WmMZ0aMJU9kewlYVc+z1PJs8Xwvub/6pp3WpN0Zz4L/IiGHEPKfDajAjlQCGYCnYSFDk/Ieri/F/mHe9GkKkAPJs9rRu9q+pXV9N5B/tcdJGJBwMET1POcgDLOJQWOLnU9LgAjSIULZP2lvd1snqLXDwNNU7m+J/3uEgbBse5yolXxAweXOSaqdzdznyrReYqFyRLIbV1svMZgtFRV0TOre9zug8LvP+6EjVRpCQlOc3uH2f+s5RE/8qzi6yH5z7zSt5fkV908raOFGNuK1sXsXpRJTKBQxd/XLIio+2XwGUvzDXQ6OE1UruZ2ZQbTSt/iHWmgGuct2D6Nd/TIZJpG4KUUXckpk7OERin6FARH7qQeitStT7RdvYmjp1lVzKV1etfar6O1w/L6zX2dwEDKQHpJRP0IgTIGPhWRDzQUXiDRSxMJwV6BscNJneQVLpUqBhMFybvA/lmBvfidPz0mMvvef4WSOAeZMClpMOOcMifgFMJIUMakIxKeOJJhfXQ2uBuKdeP0jjTOThTfA+G4kUMh8SRlDkR0JkVEReqBEL4M0xkn65+OhnmklhYAAA==",

            "AH4_H4sIAAAAAAAACu1Y227bOBD9lYDPIiBSd785XjcbwM0GVRZ9CIoFRY5sIrLoUlTSbOB/LyhZseRLbggWLTZvMjk8c2Z4OBz6AY1royasMtUkn6PRA5qWLCtgXBRoZHQNDrKTM1nCdlJ0U+cCjWicOOhSS6WluUcj4qDzavqDF7UAsR229usW67NSfGHBmg9qvxqcMHbQ2epqoaFaqEKgEXHdAfLT0A1GEg1WuM+SmSzq5ZHAfOL6zzDqQFRRADddJD5xSd+MPs9CaSFZcYQIoWE4yLG/WfZJVovpPVQ9x8EO4yAYMA67PWA3kC5kbk6ZbHjbgaobSA3jNxUaBZushvE+bh812aBeMiOh5NDjE+6uC4cZpN1SLf+FCTOtMjqvu6vpTv69zeqrBSsku6k+sVulLcBgoAvHc4bjX4CrW9BoRGySDkk7jK0Eeg67/J3K+RlbNoGOy3kBuuqc2M0WaORFrr/HfgAVr9cOmv4wmm0Ons38lUrv2Oq8NLU0UpVnTJZdPjBx0KzW8Bmqis0BjRBy0EVDAl2oEpDTItyvAI1sYg7gzVRl3ox3qaGCwwwRRkfmW4/N/JZPugJuNCsmtdZQmneKcgf13WI9yHYv4oPeG6tWIKlRK3teZTlPDayaQrnlvhHRWL8P5T5cw+HvUn6vweKiJBOeYMTHQsQu9sOE4CziESYujTm4xM3yCK0dNJOV+Su3Pio0um7laQN4PNxBEtDjHCeqWqqC6VtmwS6UXrLiT6Vu7PKuTnwF1vxuj56drcBY/t0h3Ay1QfoksoWmW5warcr5a5a7Xm/5DOZQCqbvX43wh6qz4pH7wIKGyaPBlt9RkwGHA1ZXWq6OeYoC6j2aHPM1MHrC28bOanScG9ATVs8XZiaX9nIg7cSueJu2oNbt7WM/emW2rYBBsn9/PnEV2ju8KxudUr7A91pqEKlhprY3km0SduXzMpW8WAwfe/6f7nmvNDEIwQ94hLmX5NgPQoET1w1x4BLGE8hIHrlo7RyuRd7xWjSrS6ZPZi8rRh9q+p3V9FFB/tcVJIiIcAUBTIAm2PczgpNcxBhoHgqRcBFSjtbfuu5m8xS9fhxoi8r1AxpWlygMnqguwFaynJ/MFOOLQVtGnsrOuYDSSM4KmxLrqjUYL1VdDsya8rb7lvCG77rYeqp1zjikhW1ftnXxmSdUsHbQL/Mit61X+yJt269tg/y2St/hTWxOW03esVXbKVedk37jbC+hUpX/XPthgum3k69QmTumxcmUq0LN7XadpLW+hXvUx+rhH9B8v/n2eEhpLHDmMh/7PPdxlosQkzwnEPGIMI83N9yu/vzjMZ8WNSyVKn81AR76S+U30+O+/l79LEv3JLijN/s8e4OSMiogZ5nAIkoE9hMaYAaU4NDngUvjzEsyaCpdi7uh2OiaPKPrnpc8dpkX5AmOmKDY9yKGGfcpTrxQuAQYZQBo/RPKHazZRRQAAA=="
        },
        [470] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1W227bOBD9FYPPIiDqRklvrjfJBnCyQZ1iH4I+UOTIJiKLLkW1zQb+94K62JIvSVME2Av2TR7OnDkzPpzhM5rWRs1YZapZvkTpM7ooWVbAtChQanQNDrKHc1nC/lD0R9cCpV6cOOhOS6WleUIpcdB1dfGdF7UAsTdb/22LdaMUX1mw5sOzXw1OFDvoanO/0lCtVCFQSlx3hPwydIOR0FGE+yqZ2apenyksIG7wCqMeRBUFcNNXEhCXDN2811koLSQrzhAhXhSNehx0YZeyWl08QTVIHB4wDsMR46j/D9gjLFYyNx+YbHhbQ9UbFobxxwqlYdfVKD7GHaImHeodMxJKDgM+0WFcNO6g14dq+RfMmGmV0Wc9jPYO+u930fcrVkj2WF2yr0pbgJGhL8d3xvaPwNVX0CgltkmnpB3FVgKDhH3/PsjlFVs3hU7LZQG66pPYP1ug1KducMR+BBVvtw66+G406y6e7fy9Wnxjm+vS1NJIVV4xWfb9wMRB81rDDVQVWwJKEXLQbUMC3aoSkNMiPG0ApbYxJ/DmqjK/jHenoYLTDBFGZ87bjM35ns9iA9xoVsxqraE071TlAeq71XqS7VHFJ7M3Xq1AFkZt7H2V5XJhYNMMyj33TkRT/T6Uh3ANh0+l/FKDxUWhoJkQLsfE93wcZG6Ak5BQ7LqUEio8QV2Otg6ay8r8kdscFUofWnnaAnaXO0zC8DzHSyv7b8yAnky1WWlV1Bos7q3Sa1b8rtSjRepHxp/Amt/tLbSnFRhbSn8fO1Nbb0ConTl98MJoVS7fEu76g/A5LKEUTD9ZhM5xNw1yVlTgvA34N1Vnxa6kkYcXJTuHPe2zLqeoDb0+VXCv5aZl1lNqLQfpDwsag9HQs8TbyCNeb4h9gXDnZy/CNDegZ6xersxcru0GIu3B4Q1p3h61blec/RjM8nbMhsnxkn5h39qHQj+beg1+hC+11CAWhpnarj37EjkU5s/p760y+5tkc1IhPyWFf+l/Pph/NPd8oEmMCY9jHARxhBmjHMfMy0JOQ+pmFG0/9wOwe60+7AztDHx4RuNhSKl7fhjO65LpyZVmVTWZMb0ZDW/yUoOuBZRGclbYrthsrcN0repy5NYM5MMXhz9+/cU2U61zxmFR2BG2n+SvPLTCrYP+Me/2/dr85WVpg61lZtvYdHC4PrulaT9b897tlGAH4uJhHDACgD3uBzgQ4OMkzGJMMxGTJCIUPLtcj8XzQgE3jGuVacZXsl5PGinJ6n8F/UcVFCaCc8EIFrEAHIQxxSxxY5wTArGb+AzCvBlPLW5H8SGg7ufJot5sQE8u1qCXUPKn8cPPTyJX+JGHqUhiHFCS4ywMIkxCz8vdDCJOOdr+ACoCyCoREAAA"
        },
        [471] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1WTU/jMBD9K8jnRLKTNF+30gUWqbCIdsUBcXCdSWKRxsV2WNhV//vK+aBJ24CEOOxhb8l45s2b8eRN/qBppcWMKq1maYbiP+ispKsCpkWBYi0rsJA5nPMSdodJd3SZoNgJIwvdSC4k168oJha6VGcvrKgSSHZm479tsK6EYLkBqx8c81Tj+KGFLjbLXILKRZGgmGA8QH4fusaIgkEE/pDMLK/WHQOPYO8DCl2UKApgeqQjHsGkH+V8zELIhNNiBI84vj/osdeGnXOVn72C6hUw2StgMhkU4Hd3QB9hkfNUn1Jel2EMqjMsNGWPCsWTtqt+eIjbR41a1BuqOZRsbFI8gv19GH/YX6dDkvw3zKhuBqUjsR/t7N2O20Yvc1pw+qjO6bOQBmBg6KpzraH9Fph4BoliYnp2bNL90AxIL2HXzlOeXdB1Xfe0zAqQqkti7j5BsRtg74D9ACrcbi109qIlbb9DcxFLsfhFN5elrrjmorygvOz6YRMLzSsJV6AUzQDFCFnouiaBrkUJyGoQXjeAYtOYI3hzofSn8W4kKDjOENlo5LzJWJ/v+Cw2wLSkxaySEkr9RVXuoX5ZrUfZHlR8NHvt1QzIQouN+Xx5mS00bGrd3HFvh2gqv4ZyH67m8LPkTxUYXOTjlDjRitoBC7DtrZLUjqKQ2AlLXOZQ308dirYWmnOlf6Qmh0LxfTOepoA37ZlEE3+c42lRwcmdkGuDdS3kmhbfhXg00Z1q3AGt343dUK9L8Uhg1KXzWWgpyuyIF3Z7XnPIoEyofB1z/CaqVXE8oeNHbw4j2fou46kar6Xkm7FMwcRx31zGcg2c3snW+pm5mqYa5IxWWa7nfG30nTQH+wNXb/ZKNgvEPPSk8Yj+ucEkOlyQ7yw3s5W7L7+77Vt4qriEZKGprsyOMWv//wj8iyNw+ck776nLCkLM3MS3MSaJ7UVeYK8wTW03SkM3DT2XYUDbh05e2l/D+zdDozDmvdGzVk3uvYA8nEwLlsOaM1qc3IISlWSghuKGcUjCNGU280PP9oib2CvXSe3AZQlgFtIIGNr+BYDIiIsECwAA"
        },

        #endregion

        #region A Rank | Sequence | Timed | Weather

        [472] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XS2/bOBD+KwbPIqD36+Z402wANw3qFDkEPVDU0CJCiy5FpfUG/u8F9YglP+IgyGJ72Bs9nPnmm9HHIf2MprWWM1LpasaWKH1GlyXJBEyFQKlWNVjIbM55CbvNvN+6zlHqxomFbhWXiusNSh0LXVeXv6ioc8h3ZuO/bbE+S0kLA9YsXLNqcMLYQlfru0JBVUiRo9Sx7RHy69ANRhKNIuyzZGZFvTpRmO/Y/hlGPYgUAqjuK/Ed2xm6uedZSJVzIk4QcdwwHPXY78I+8aq43EA1SBzsMQ6CEeOw/wbkERYFZ/qC8Ia3MVS9YaEJfaxQGnRdDeND3CFq0qHeEs2hpDDgE+7HheMOun2o4v/AjOhWGX3W/Wh3r/9eF31XEMHJY/WJPEllAEaGvhzPGtu/ApVPoFDqmCYdk3YYGwkMEvb9u+DLK7JqCp2WSwGq6pOYj52j1Its/4D9CCrebi10+Usr0h080/k7ufhJ1telrrnmsrwivOz7gR0LzWsFn6GqyBJQipCFbhoS6EaWgKwWYbMGlJrGHMGby0q/G+9WQQXHGSKMTuy3GZv9HZ/FGqhWRMxqpaDUH1TlHuqH1XqU7UHFR7M3Xq1AFlquzXnl5XKhYd0Myh33TkRT9TGUh3ANh28l/1GDwUWeE0GSxQxTP/Cx73kME5fa2AkojRwGWWh7aGuhOa/0F2ZyVCh9aOVpCnghmCSnGU6FmLSh+zRvpFoR8beUjwaonxj3QJrfxl6BfjmLjIgK+rPZbZoC+1PamVp434nMJOoxF1rJcvkBqLY3QJ3DEsqcqM1uWL8R4S9ZZ2K/0tbDDZMXhwPahy4jDke8vlVwp/i6ZdZTai1nGj0GiwLXEG8jz7bzldgR4S+l2NwXUN5IPaWaP8FCnGhcD2LOzpRpUDNSLws95ytzaTntxv6hap4rtWpvRbMYjP8jM96LguTwmn/lxjZPjX669TL+Cj9qriBfaKJrc3Gat8wJbb9Nq+e1999K7Kia3iSbs/r4dyXw3m8+nKCM+pA5CfYzFmLfpglOGA0wie0wDz3qUi9A2+/9CO3euw8vhnaKPjyj4Tj1gyh2Tw/UeV0SNbmXgjETNJyqzmvtuc6h1JwSYXpicrUO05Wsy5GbYZDsv1i88esxNplqxQjtzuzxd3OQBGfebcHWQn/M34DdLfzuu9cEG8vMdLVp6PA27u5gs2zNO7dj6h0qjTIWOl6AvYwy7BM3wElAbexSFsdhRkgWxGhrHSrJO13ArSCal/VqcsFpwdWfI6X/tVN+qHZcL2QZibDjJB72AzvESQw2DkMnCiDwYpIkzZRqcTuKD37kfp9M08mNVLr4SVQ+uaRSyKX58JNFrZ5gM35PEgouy7IM+1HmYd8OCU48FmHfTkhEIMscCNH2N71unEloEAAA"
        },
        [473] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XTW/bOBD9KwbPIiCJlET55nrTbAA3G8Qp9hD0MKJGNhFZciiq22zg/15QH7Hlj2QbGNgecqNHwzdvho/D8TOZ1KacQmWqabYg42di1zNVoF1fFJDkmJKx0TU65ColY1/EDrnRqtTKPJGx55Cr6uKHzOsU063Z+m+cButLWcqlBWsWvl01OKFwyOX6bqmxWpZ5Ssae6w6QX4duMOJosMN9k8x0Wa9OJMY9l7/BqAcp8xylOY3j7e7y3yZV6lRBfgIv9PgAj3e7PqtqefGEVV9Q7rnBHv8gGPAP+xOBB5wvVWY+gWqysIaqN8wNyIeKjIOuxqE4xN1FjTvUGzAKC4k7fML9feGwnn6/Vat/cQqm1ckx0YXiAMzfOxzWgd0tIVfwUH2G76W2eANDnx1zhvZblOV31GTs2ZqdoMAHAftyflKLS1g1eU+KRY666oPYo0/JmEUuP2A/gBKbjUMufhgN3SW0B3FXzv+B9VVhamVUWVyCKvraUs8hs1rjF6wqWCAZE+KQ64YEuS4LJE6L8LRGMraFOYI3KyvzbrwbjRUeZ0goOfG9jdh83/KZr1EaDfm01hoLc6Ys91DPlutRtgcZH43eeLUCmZtyba+vKhZzg+umi265dyKa6PNQ3oVrOHwt1GONFpegiz76vqAs4THlXpDQRDKXZiJIBAhPYCjIxiEzVZm/MhujIuP7Vp42gReCcXya4STPR+3WfZrXpV5B/mdZPligvoH8jdD8tvYKzctdzCCvsL+b3UebYH9LO1MLz73INqYec250WTQ3679ud9nO9hkusEhBP52BVwP8R1kn+X6mrYcfxi8OW9onXY5R2/W602p9KlIU+OzF5VSsgdMr0To/q+1JZlBPoV4szUyt7BvjtR/2Rd/MGrVuHzG72GnPR3owi4L48I1+5X21c0LffXqZ3eJjrTSmcwOmtu+cHUROaO8NLf2qZD4k8GsSeO+Z73Q4L/KlSLyYQspCytEPaeJHHvUyDFkCaeLzmGy+9S2uG1bvXwxtl7t/JrvtjgeRiE43vFldgB5NJKS4UlJBMWh83msVukqxMEpCbsvSTuPWYbIq62LgZknE+0MFG857wkaqdQYS57ntRz37OHhjlgo2Dvlt5vbty/ju99ButpapLWNTwd0XsnsX7bI1b92OKXZXXUGUxDJNqJ+lknKfCQoZ45RJP+RuGGVenJCNc6gecTqB+YNar1WxGM0N6LMr59gflg8h/e9CQggTEUSScgwiyiFOKYg0pBlLWYgJCD9lx4RkK/Z6G7oFBSuofp8WdFyBH0I6j5BE6rkySpD6EMWUS8moiBApCyUGCNyVKI52pOBDSB9C+roVEojUFTJkNPVFSDl3PSpSSKh0AxmwRCa+K5vBqcXtKN7ziH0bTcajW9A4mjzWYJQc2f/DaoWFFc9OiIxxGUcippJ5MeUMBI0ROI2DJAvAl0yKkGx+AvhfKKGiFAAA"
        },
        [474] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1WTW/bOBD9KwbPIiBK1OfN8SbZAG42qLPYQ1AsaHJkE5FJl6LaZgP/94KSFUuKnTTdHBbY3iRy5s2b4eMMH9G0tnrGKlvNihXKH9G5YssSpmWJcmtq8JDbnEsFh03RbV0JlAdp5qEbI7WR9gHlxENX1fk3XtYCxGHZ2e9arA9a87UDaz4C99XgxKmHLre3awPVWpcC5cT3B8gvQzcYWTLw8F8lM1vXmxOJUeLTVxh1ILosgdsuE0p80jcLXmehjZCsPEGEBHE8qDHdu13Ian3+AFUvcDRiHEUDxnF3BuweFmtZ2DMmG95uoeoWFpbx+wrl0b6qcfoct4+a7VFvmJWgOPT4xGO/eFjBoHM18h+YMdsqo4s69g5G9Q/33rdrVkp2X12wL9o4gMFCl07oDdc/AtdfwKCcuCIdk3acOgn0Anb1O5OrS7ZpEp2qVQmm6oK4wxYoDxOfPmM/gEp3Ow+df7OG7S+eq/ytXnxl2ytla2mlVpdMqq4emHhoXhv4AFXFVoByhDx03ZBA11oB8lqEhy2g3BXmCN5cV/an8W4MVHCcIcLoxH4bsdk/8FlsgVvDylltDCj7TlmOUN8t16Nsn2V8NHpj1QpkYfXW3VepVgsL26ZRHrjvRTQ170O5D9dw+FPJzzU4XMTTJROEFjgtaIJpIAqcciBYAM2YSMMCaIJ2HprLyv5RuBgVyu9aeboEnghm2WmG07KctK5jmtfabFj5u9b3DqjrGH8Ba/7bS+h2K7Auk+467pdaHEoS13I654U1Wq3e4u6HPfc5rEAJZh7ejPCbrpflE/eBRRBnTwYHfidNBhyOWN0auT0VKYmC8MnkVKyB0QvR9nZOrdPCgpmxerW2c7lxY4K0G2MZNw+E2rRzyH30Gm7bC6NsPD9eHM1umncNpFPKR/hcSwNiYZmt3Wxyz4WxfH5MJT8shl9n/q/OnL7xzHtNivDCzzhwnAWuScXJEjOABBMifCESUoR+jHafui61f1LePS20jeruEfU7Fo2SLDzdsy4NU2IyM/qrskyWkzOwlg36F3mpSlcClJWcla40LmRrMN3oWg3MHJFs/DYIh++01EWqTcE4LErXhLoMnl2k8ZMo2nnoP/PCPgy4nx5rztmtzFwZmwr2Bx3KEVNa/X1HE4qDT5NpPrnQWlS2LorJ+QbMChR/QC1M63gAOqbrngbd6zXylwRnEfMx5WmIlxlhmBTAlxH3KU19tPOeayw4neK8VsxMFrKyYCrOymG+v/T1f9IX88OCLlmKyZJTTANKcAqMYb9gkaBhwUnImx7X4u6TaJiELzAZRIAAgtjHJCtSTIMswGkhOKaZT0PGYkH9CO2+A5JZ6HcDEAAA",

            "AH4_H4sIAAAAAAAACuVWTW/bOBD9KwbPEiBR37o5rpM14KZBnWAPQbGgxZFNRBZdkkqaDfzfC4piLTm20xZBscDepOHMmzejpxm+oHGj+IRIJSflCuUvaFqTZQXjqkK5Eg04SB/OWQ37Q2qPZhTlOM0cdCMYF0w9o9x30ExOvxVVQ4Huzdp/Z7A+cl6sNVj7gPVTixOnDrra3q4FyDWvKMp9zxsgn4duMbJkEOG9SWaybjaWQeh74RsUbBSvKihUL9Dvu+G303JBGak0wLiq+NOnRxAF2c4mtrNHGu3jOB60OuzALplcT59B9uhEB3VE0aCO2H4K8gCLNSvVBWFtNdogrWGhSPEgUR51zY3T17h91KxDvSGKQV1Aj098GBcP+4ptqGD/woQoIxCb9TAaH3yVoIu+XZOKkQd5SR650AADgy0ncIb2z1DwRxAo93WTjik8TrUweglt/y7Y6ops2kLH9aoCIW0SLQGK8iDxwlfsB1Dpbueg6TclSPf/6c7f8sUT2c5q1TDFeH1FWG374foOmjcCPoKUZAUoR8hB1y0JdM1rQI5BeN4CynVjjuDNuVS/jXcjQMJxhshFJ85NxvZ8z2exhUIJUk0aIaBW71TlAeq71XqU7auKj2ZvvYxAFopv9f/K6tVCwbadl3vunYjG4n0o9+FaDnc1+9qAxkXpEvvgRYFbQFa4YURSN8XR0k08XPpRGi7jCNDOQXMm1adS55Aovzfy1AX8IJhlpxmOq2pkQg9pXnOxIdVfnD9oIDsx/gbSvpufUJ9KULoS+zt2JoMT+okeOTZ4oQSvV78S7gW98DmsoKZEPP8ywp2ED7zp/K2jsdiCBmE41rSNw570SZcBsSNedxJuBdsO0xvL8fRJhDVt43KKwMDpDIXOT+t6XCoQE9Ks1mrONnqh+ObgUPDtjaIRZmPph95oNlMzyl5v4jNLVa9/O2qspj7D14YJoAtFVKO3mL5fHArt5/T007I55vjnhPC/++a9cVb6EKRpjN0IU+yGcQLukoa+S3GRFRnECQ4o2n2x86y7g97/MJiRdv+C+rMtjJIsOD3drgSp6Wgi+FOtCKtGF6AUGUw6/1yXZhRqxQpS6dbolMZhvOFNPXDTRLLDW0QwvNGlOlMjSlLAotLjylaQRW9cnqKdg/4zV/L9KvztBaiDtWWi22jk+ES2Zi1KOx77WxLliNS8/uc+TEI3+DIa56NLzqlUTVmOphsQK6iLZ9TH6WEfkXpPllkZRDjCvptkPnFDL6Mu8bLEjeIkDgHihC6hlaXB7epqmeAzTHoZAi+muEhD1/N9zw1L7LupF4MbLpewTMOUZhlFu+9nLcou5w0AAA==",

            "AH4_H4sIAAAAAAAACu1WTW/jNhD9KwbPEiDqg/q4Oa6TBvCmwTpFD8GioKSRTUQmvRS1WTfwfy8oibak2M4myKGH3iRy5s2b4eMMX9C0VmJGK1XNihVKXtCc07SEaVmiRMkaLKQ3F4zDcTM3W7c5StwottC9ZEIytUMJttBtNf+ZlXUO+XFZ2+9brC9CZGsN1ny4+qvBIZGFbrYPawnVWpQ5SrDjDJAvQzcYcTjwcN4kM1vXG8PAx47/BgXjJcoSMtVzxH0z9+2wQuaMlmdKil1CBkX1O7drVq3nO6h6gYMR4yAYMCam6PQJlmtWqCvKGt56oTILS0WzpwolQVdGEr3G7aPGHeo9VQx4dk4aPnbIGIYMC+oaJMn+gRlVrTIMibG3OzoOr/N+WNOS0afqmv4QUgMMFkx2njVc/wqZ+AESJVjX7JS0SaQV0QtoynnFVjd00+Q95asSZGWC6LPPUeKFjv+K/QAq2u8tNP+pJO0unj6IB7F8pttbrmqmmOA3lHFTDxtbaFFL+AJVRVeAEoQsdNeQQHeCA7JahN0WUKILcwJvISr1Ybx7CRWcZohsdGa/jdjsH/kst5ApSctZLSVw9UlZjlA/LdeTbF9lfDJ6Y9UKZKnEVl9fxldLBdumUR65dyKays+h3IdrOPzJ2fcaNC4qojTzgsKx09jBtp9GsZ36hW/7uefHDjgkIATtLbRglfqj0DEqlDy28tQJHAjG8XmG07KctK5jmndCbmj5uxBPGsg0kL+ANv/tJdS7FSidibmO3VKL4+NQdyDjvFRS8NV73B2v576AFfCcyt27EX4TdVoeuA8sXBIfDI78zpoMOJywepBsey5SGLjeweRcrIHRhWidnVbrtFAgZ7RerdWCbfTUwO3GWMbNA6GW7VjSH72Ge6KremEQj6fLxUmth7vpJ0Y4X+F7zSTkS0VVrSeXfj2M1fRrovllbfwvgQ9JwJy5/84z7/Ush+ZuFIS5jSkubB+7sZ16OLBJTiglGIrM8dH+m2la3Qvz8bDQ9q3HF9RvYH4Qxt75FnYjKc8nMymeuaKsnFyBUnTQzvClKt3mwBXLaKlLo0O2BtONqHnP7NSrKYjHLwdv+KiLdOBaFjSDZalblEno1b0aP5iCvYX+M+/v4/j78NDTznplpqvaqvOZbttRWJmi9icjShDlgv/96Ie+7X6bTJPJtRB5peqimMw3IFfAsx3q4/SwTyi/p9LQdYosCIidFyS1/aLAdoyL0AYS0hgiB3DgNiptcbu8Gib4ApNeBJy7hISxZ2cx9W3fJYEd5RDaJC3S1MNeQYGg/b+fzZes1A0AAA==",

        },
        [475] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1Wy27bOhD9FYNrERAl6kHtXN80DeCmRp2LLoIuKGpkE1Ekl6LSuoH//YJ6xJItJWjgxV10Jw2Hh2eGhzPzjOaVLha81OUi3aDoGV3lPM5gnmUo0qoCC5nFpczhuJh0SzcJipyQWWilZKGk3qOIWOimvPolsiqB5Gg2/ocG63NRiK0Bqz8c81Xj+KGFrnd3WwXltsgSFBHbHiC/Dl1jsGCww36TzGJbPXYMKLHpGxS6XUWWgdATGaHEJv1dztssCpVInk3gEcf3Bzmm7baPstxe7aHsBeCdBOB5gwD87g74A6y3MtUfuKzDMIayM6w1Fw8lirw2q354jttHZS3qimsJuYAeH/90nz9MqNNtVfI3LLhulDEmMz88A3NObsdtwe62PJP8ofzInwpl8AaGLjrXGtq/giieQKGImJxNUKCDA7t0fpCba/5Yxz3PNxmosjvE3H2CIjew6Rn7AVR4OFjo6pdWvH2H5iLuivVPvrvJdSW1LPJrLvMut5hYaFkp+AxlyTeAIoQsdFuTQLdFDshqEPY7QJFJzAjesij1u/FWCkoYZ4gwmlhvTqzXj3zWOxBa8WxRKQW5vlCUJ6gXi3WU7VnEo6fXXo1A1rrYmecr881aw66um0furYjm6jKU+3A1h39z+aMCg4u4KxwunBiThHFMXZLi0KYpdgh4sesAc4IEHSy0lKX+kpozShTdN/I0AbwQZGya4TzLZs3WU5q3hXrk2aeieDBAXQH5Brz+N/YS9MtbTHlWQvc220UTYPdKW1MDT4kThCYTLehaqyLvtbg/3b+EDeQJV/sLMLNN7P8UVZydxtp4OD57cTjynnQZo9b3ulNyN3VS4Dnui8vUWQOnV05r/Yy656kGteDVZquX8tF0GdIsnMq+ni8q1bQx89Er0CNV2A08dt6mX2mxZjbo6k8ntK/wo5IKkrXmujKdzgwfE+rr7ixgo2Iau9nXJPNXAn8mgffeea/GMUISRlmA7UBwTKlwMCOhh5kXgk19QnjqosP3rsi1A+r9i6Gpc/fPqF/wqBewcLrkXfOMCy3F7BP/PazO5LXs3CSQayl4ZlJijmoc5o9FlffcxqZOj51OGO5w+DPFbF2plAtYZ6Y0dYEw743ByjtY6H8zth/b5Lubo9lsLAuT1Tqh/XbZNknz2ZiPbmPi7QnNI27g+5ThwHUopoIkmLspxTSwY4fZPHQpoIN1LiR/OoBVke13VTmbqxJyKeTFtfRu8YyL8K+WVhfRUsxInHiEY+bbDqY+xJj7qYuT2A18lwmf+KQuWg1uS/GeBt732TyarRQIWcLsG9egZutKPcF+OPl5sbCp7ac4IR7BNHEAMz8NcBLHIfVs5opYoMN/tqqASCEQAAA="
        },
        [476] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XS2/bOBD+K1pe9iItqBcl+ea4STaAnQ3qLHoIeqCokU1EJh2KapoG/u8L6mFLsp1uixx66E0eDr/5Zjgvv6JppeWMlrqc5Ss0eUWXgqYFTIsCTbSqwEbmcM4FHA6z7ugmQxMvTmx0p7hUXL+giWujm/LyKyuqDLKD2OjvGqyFlGxtwOoPz3zVOCS20fX2fq2gXMsiQxMX4wHy29A1RhINbuDvkpmtq03HIHBxMKKQjCh0t2RRANNnIhK42O3f8r7PQqqM0+IMnusRMohx0F674uX68gXKngPhyIEwHDhAujegj7Bc81xfUF67YQRlJ1hqyh5LNAnbqJL4GLePmrSod1RzEAx6fMj4HhkG1OuuKv4NZlQ3mXEqzUh8BOaNXsdvwe7XtOD0sbyiX6QyeANB551vD+UfgckvoNDENTE7QyEYGOzCecFX13RT+z0VqwJU2RnxbPSJ6/WyoNsO6gSyH+HgyLmBpXi3s9HlV61oW6bmne7l8plub4SuuOZSXFMuutA7ro3mlYIFlCVdAZogZKPbmiO6lQKQ3SC8bAFNTNxO4M1lqX8a705BCacZIgedOW8s1ucHPsstMK1oMauUAqHfycsR6rv5epLtkccnrddaTf4stdya6uZitdSwrdvqgXubY1P1PpT7cDWHfwV/qsDgIsZI4AEFx3P9zAkiHDlxnHiOy3IIWZblXhqhnY3mvNT/5MZGiSYPTXoaB/YEk+Q8w2lRWM3VMc1bqTa0+FvKRwPU9ZdPQOvfTSWZ0xK08aSrqQUXRnrPN6aeveAvbKMF/dqXxUZmuv9Q1/X38oG+W+u3php+getFsQlly2qplRR1bbZq+0LPaVHuC/8E3REs9nuoc1iByKh6eS/gD7JKi30IBxoeSfYKR94cq5yi1te6V3x7zlIUev5e5ZytgdIb1lo9UzTTXIOa0Wq11nO+MbPNbQ7G1VRvNZVqhqf56I2Fkx06TMYz8M31wmwkXVvr8vcjPFVcQbbUVFdmvpqVZ5zUozeLkrdS7H+nzO8U+LEU6N48+ME377XOEDOMkyxyfHCJE8Q4cBIWgxMRxkgeZsQPE7T73PXOdi1+2Aua9vnwivp9NAhj7I876dTa0JXgOWcgtJVzYaUKKFtDaek1WM9Ug/qztMpK5ZTBH4fG+4GqR2um5LPQlBfWBWhNBz3YfSumNxkIzRktTCANwUZhupGVGKgZ2sl4v/GHm6nppMuGYLMsdf4eld146wt3Nvpl/lMchvRPj2Zz2UhmJox1BPvDuh3R5rMRH9RO5XgvH2nsu17kBQ4JqOcEWR47MfNjh6V5SuIQkwibUT7Ot+ityb2QUijJHq0ZFdnLr5M6p/6V/c4k+50yiWQYAwbXYZTmThClsZMSEjp+4ocMeymNMlx3tga3pfgQROSzNZ1YV1yAc61oBtb0qaKaM+tOSQZlycXKWphuxWlhlsGezThNGcHEd1KaZU4Q4MBJGSEOJiTJwyTHMc7Q7j+2nCEIzxAAAA=="
        },
        [477] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XS2/bOBD+K1pe9iItqBcl+ea4STaAnQ3qLHoIeqCokU1EJh2KapoG/u8L6mFLsp1uixx66E0eDr/5Zjgvv6JppeWMlrqc5Ss0eUWXgqYFTIsCTbSqwEbmcM4FHA6z7ugmQxMvTmx0p7hUXL+giWujm/LyKyuqDLKD2OjvGqyFlGxtwOoPz3zVOCS20fX2fq2gXMsiQxMX4wHy29A1RhINbuDvkpmtq03HIHBxMKKQjCh0t2RRANNnIhK42O3f8r7PQqqM0+IMnusRMohx0F674uX68gXKngPhyIEwHDhAujegj7Bc81xfUF67YQRlJ1hqyh5LNAnbqJL4GLePmrSod1RzEAx6fMj4HhkG1OuuKv4NZlQ3mXEqzUh8BOaNXsdvwe7XtOD0sbyiX6QyeANB551vD+UfgckvoNDENTE7QyEYGOzCecFX13RT+z0VqwJU2RnxbPSJ6/WyoNsO6gSyH+HgyLmBpXi3s9HlV61oW6bmne7l8plub4SuuOZSXFMuutA7ro3mlYIFlCVdAZogZKPbmiO6lQKQ3SC8bAFNTNxO4M1lqX8a705BCacZIgedOW8s1ucHPsstMK1oMauUAqHfycsR6rv5epLtkccnrddaTf4stdya6uZitdSwrdvqgXubY1P1PpT7cDWHfwV/qsDgIsZI4AEFx3P9zAkiHDlxnHiOy3IIWZblXhqhnY3mvNT/5MZGiSYPTXoaB/YEk+Q8w2lRWM3VMc1bqTa0+FvKRwPU9ZdPQOvfTSWZ0xK08aSrqQUXRnrPN6aeveAvbKMF/dqXxUZmuv9Q1/X38oG+W+u3php+getFsQlly2qplRR1bbZq+0LPaVHuC/8E3REs9nuoc1iByKh6eS/gD7JKi30IBxoeSfYKR94cq5yi1te6V3x7zlIUev5e5ZytgdIb1lo9UzTTXIOa0Wq11nO+MbPNbQ7G1VRvNZVqhqf56I2Fkx06TMYz8M31wmwkXVvr8vcjPFVcQbbUVFdmvpqVZ5zUozeLkrdS7H+nzO8U+LEU6N48+ME377XOEDOMkyxyfHCJE8Q4cBIWgxMRxkgeZsQPE7T73PXOdi1+2Aua9vnwivp9NAhj7I876dTa0JXgOWcgtJVzYaUKKFtDaek1WM9Ug/qztMpK5ZTBH4fG+4GqR2um5LPQlBfWBWhNBz3YfSumNxkIzRktTCANwUZhupGVGKgZ2sl4v/GHm6nppMuGYLMsdf4eld146wt3Nvpl/lMchvRPj2Zz2UhmJox1BPvDuh3R5rMRH9RO5XgvH2nsu17kBQ4JqOcEWR47MfNjh6V5SuIQkwibUT7Ot+ityb2QUijJHq0ZFdnLr5M6p/6V/c4k+50yiWQYAwbXYZTmThClsZMSEjp+4ocMeymNMlx3tga3pfgQROSzNZ1YV1yAc61oBtb0qaKaM+tOSQZlycXKWphuxWlhlsGezThNGcHEd1KaZU4Q4MBJGSEOJiTJwyTHMc7Q7j+2nCEIzxAAAA==",
        },
        [478] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1WTW/bOBD9KwZPu4AESBQlUbq5XjcN4GSDKPuFoAdaHNlEZNElqaZp4P++oD5iybGTNshhD3ujyOGbN8OnmXlE09rIGdNGz4oVSh/RvGLLEqZliVKjanCQPVyICvaHvD865yjFNHHQlRJSCfOAUt9B53r+LS9rDny/be13LdaFlPnagjULbFcNTkQddLa9WSvQa1lylPqeN0J+GbrBSOLRDe9VMrN1vTkRGPE9csAIHzDqQWRZQm76SIjv+UMz/DoLqbhg5QkiPo6iUY5Jd+2j0Ov5A+iB4/CAcRiOGEf9G7A7yNaiMB+YaHjbDd1vZIbldxqlYZfViD7HHaImHeoVMwKq/JRSiO9FhzDROKG4R1LiO8yYaYXSk4heeY6gu32zZqVgd/oj+yqVBRht9NEFznj/GnL5FRRKfZuzY0qPqFXEwGGfzg9idcY2TdzTalWC0r0T+/YcpUHskWfsR1B0t3PQ/JtRrPsP7UPcyOyebc8rUwsjZHXGRNXnw/UdtKgVXIDWbAUoRchBlw0JdCkrQE6L8LAFlNrEHMFbSG3ejHelQMNxhshFJ85bj835nk+2hdwoVs5qpaAy7xTlAeq7xXqU7bOIj3pvrFqBZEZu7e8rqlVmYNvUzT33TkRT9T6Uh3ANhz8q8aUGi4twwRNK8dL1ebB0SRGE7jIJuMswDlnBAWNO0M5BC6HN74X1oVF628rTBvBEMElOM5yW5aS9ekjzUqoNKz9JeWeB+gLyF7Dmu/0J7akGYyPpf8duq8UhfmwrUH85M0pWq5+57gWD6wtYQcWZevhphN9kvSyfuI8scJQ8Gez5nTQZcThidaPE9pSnOMTBk8kpXyOjF7x1dlat08KAmrF6tTYLsbFdw28PDmXczAu1atuSXQwK7pGqGsRhcthnk5cat+31fT3phXMNX2qhgGeGmdp2LjtMHKrpx0Tzw9r4XwJvksCpN39xWNuNalZBMME0TNwER7lLeBK4SUGoy32CE4w55XiJdp/7otUNnLdPG23dun1EwwJGQorJ6RK2qCum9L1UfJJVVnXDSua/lKBzDpUROSttVqy31mC6kXU1MDs2MIXJ4dAQjOc5ah3XqmA5ZKWtTn0sSfjKrBTuHPSfmcT3ne/N/S67Z1u7M7NZbRI67IBd37PLdntvdky/A61B7OckDKkbEsAuWfqRS/3l0o09TvI8DjDzcaO1FrejeEti+nky/zudTLWWygCfTMt8DRurg8kFM6AEK/Xkl0/z6Z//TK4vz34dt2VKC+JHhecWMTCXJNR3Kc2xW+BlERMa4hw8tPsX8z2x0r4NAAA=",
        },
        [479] = new List<string> { },
        [480] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACs1X247iOBD9FeTnZBXnhpM3hqV7W4Le1sBoV2rNg5NUwCLEjO3MNNvi30fOBZJA0zDqkfbNlF2nTlXqxisaFYqPqVRynC5R+IomOY0yGGUZCpUowED6cspyOF4mzdVDgkKbBAZ6EowLpnYoxAZ6kJOXOCsSSI5i/X5fYc04j1carDzY+lTi+MRA99vFSoBc8SxBIbasDvJl6BIjGHY0rHfJjFfFpmHgYst9h0KjxbMMYtVSxO1n9vtmuUgYzd4IKbZ9vxNUt1a7Y3I12YFsGfZ6jD2vw9hvgk7XMF+xVH2irOStBbIRzBWN1xKFXh1Gn5zitlGDGvWJKgZ5DC0+fl/P70bQblQF+w/GVFWp0Fjta9u9+Du19mJFM0bX8o5+50IDdASNO47RlX+GmH8HgUKsg3Qul32iU6BlsInfJ7a8p5vS0VG+zEDIxoj+2AkKnaHlnrDvQJH93kCTFyVoXWk68gs+/0G3D7kqmGI8v6csb+JhYgNNCwEzkJIuAYUIGeixJIEeeQ7IqBB2W0ChDswZvCmX6pfxngRIOM8QmeiN+8pieX/kM99CrATNxoUQkKsP8rKH+mG+nmV74vFZ6+WrKkHmim91vbJ8OVewLTvjkXudRCPxMZTbcCWHLzn7VoDGRZEFkPiRbfqpRUw3op4ZBD41LdunEA8tgnGA9gaaMqn+TrUNicLnKj21AweCQfA2w1GWDSrVPs1HLjY0+4vztQZqOsY/QMvfVRHqWwlKe9KUYy2qcFw81C2nUZ4rwfPlLeqW01KfwhLyhIrdzQh/8iLKDtz1iwXbgOh1khnLD1e67v+wDDSjLy2Zg7WsY8H2g4OBo3/1k4OBlGYSLmh2XLte+YuEhWDbKg6NG5Xk9zg79Gwdz8rEje52dG93uFbX5TlKFYgxLZYrNWUbPRdxddGv23IFKkQ1ePWhNWHOjBFn6AX9+XlxF9HrS9NAm0r5DN8KJiCZK6oKPZv1ftQvn+uq5OpiuC7nr8vZ65Kz/eo04a5NmGsz4zelQPPN3Ru/eatJp6kT2F6ATRw5iemmaWwSYoHpARCS+lHsQYD2X5suXe/QzwdB1aifX1G7Y7vekFzo2TPOc8Hj9WBM82TXadz4UngeEsgVi2mmY6JtVQ9GG17knWeaQdBfipzugkq0pUKkNIZ5prvv2Y3YPS2o/mro7Q30v/lrcRz0vzze5z/oVkvGOqplQNsDvx7z+liJj8/OZW8r02zbw5blpibxbdt0PUxN4tiBaTt+RIAMncj3ykyrcGuKzy6xvg4m/4aDO84TqYo0HUw2IJaQxzp1WgZwEniOSx3Tcq3EdCMMJokC17QtjD0nIF7iDtH+J/OqF955DgAA"
        },
        [481] = new List<string> { },
        [482] = new List<string> { },
        [483] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XTW/bOBD9KwbP4kIUqc+b602zBpxsUGXRQ9ADJY1sIrLoUFTbbOD/XlCyYsmWnTYIsLvY3GRy5s2b0dPM+AlNay1nvNLVLF+i6AldlDwpYFoUKNKqBguZy4UoYX+ZdVfzDEVOEFroRgmphH5EEbHQvLr4nhZ1Btn+2NhvW6wrKdOVAWseHPPU4HiBhS43tysF1UoWGYqIbQ+Qz0M3GKE/8LBfJDNb1esTiTFisxcYdSCyKCDVXSaM2KRv5rzMQqpM8OIEEeJ43qDGbOf2UVSri0eoeoHdA8auO2Dsde+A30O8Ern+wEXD2xxU3UGseXpfocjdVdULjnH7qOEO9YZrAWUKPT7eoZ83rKDTuSrxN8y4bpXRRT30dg7qT3fetyteCH5ffeRfpTIAg4MuHWoNzz9BKr+CQhExRRqTthcYCfQCdvX7IJaXfN0kOi2XBaiqC2JedoYi6tvsiP0AKthuLXTxXSu++/BM5W9l/I1v5qWuhRayvOSi7OqBiYUWtYIrqCq+BBQhZKHrhgS6liUgq0V43ACKTGFG8Bay0q/Gu1FQwThDhNGJ+zZic7/nE28g1YoXs1opKPUbZXmA+ma5jrI9yng0emPVCiTWcmO+V1EuYw2bplHuue9ENFVvQ7kP13D4qxQPNRhclPsJzxIKOA9oglluMxx6lOM8Y0liM845A7S10EJU+s/cxKhQdNfK0yTwTDAMTzOcFsWkdT2keS3Vmhd/SHlvgLqO8Rl487v9CM1tBdpk0n2Ou6MWhxHftJzOOdZKlstfcbdpz30BSygzrh5/GeF3WSfFM/eBheOFzwZ7fidNBhxGrG6V2JyK5LsOfTY5FWtgdCbazs6odZprUDNeL1d6IdZmTJD24lDGzYJQq3YOmYdewx3pqtR3w+PBemZGmuHe9ZNOOJ/goRYKslhzXZtRZbaHQzX9nGh+WhvvEniVBF77zns9i6Wc+q5HMFBiY5b6Kebc8TGHjIa+nzLwHLT90jWt3YZ593zQ9q27J9RvYMz1zTJyqoVdSVl+W4nNoIORc4WZZ1BqkfLCVMNEaQ2ma1mXAzMTOzzcDuhwUwtMpFrlPIW4MG1ofEd1Q/eFHcndWuhfs3LvJ96r55xxNiczU9WmoP3Jt5t35rE93puN6banscwH6jm+h7ntBZjZqYM54xQHGUuYS7M8ZBnaWkcacs+MwUVdcjWJU6k2QpbvSvp/KIkxFgYhEGx7iYsZz2ycJDzAPLAhy0PqEGqPKKn5u3NeSdMS1oeZ/KNC+i8qp3vtnfqPlWRK4Zx+F7GGouBq8lmqNRrV0V40VRflQFpTTKLJ9KHmWqSTeVmZfw9Clr+R10nOSQKaJ7aNKacEMzdLcWBTG2cZyzwe5CQENiq5M83rRhaPm7qaxHWR10oMV/l31b2rDvkBIcA9igFsDzOwAxwmPMehl5M8SQkJbNqsZS3uLpE7FlDsfJlMx6hM5sN/qzwMXJe4KXYISzELgxzzAACT0M9Cj4DDEh9tfwBBfpH9xhQAAA==",

            "AH4_H4sIAAAAAAAACu1XTW/jNhD9KwZPLWAWFKkv+uZ1s6kBZxvEKXoI9kBRI5uILDoUtbtp4P9eULJiy5ad3SCHLZqbTM68eTN6mhk/oXFl9USUtpxkCzR6QheFSHIY5zkaWVPBELnLmSpgd5m2V9MUjWjMh+jaKG2UfUQjb4im5cU3mVcppLtjZ79psK60lksHVj9Q91TjhPEQXa5vlwbKpc5TNPII6SCfh64xeNTxIC+SmSyr1YnEfI/4LzBqQXSeg7RtJr5HvH0z+jILbVIl8hNEPBqGnRr7W7ePqlxePEK5Fzg4YBwEHcZh+w7EPcyXKrMfhKp5u4OyPZhbIe9LNAq2VQ3jY9x9VL5FvRZWQSFhj0946Bd2K0hbV6P+gYmwjTLaqIfe9KD+bOt9uxS5EvflR/FFGwfQOWjTYcPu+Q1I/QUMGnmuSH3SDmMngb2Abf0+qMWlWNWJjotFDqZsg7iXnaIRi4h/xL4DFW82Q3TxzRqx/fBc5W/1/KtYTwtbKat0cSlU0dYDe0M0qwxcQVmKBaARQkP0qSaBPukC0LBBeFwDGrnC9ODNdGlfjXdtoIR+hgijE/dNxPp+x2e+BmmNyCeVMVDYN8ryAPXNcu1le5Rxb/TaqhHI3Oq1+15VsZhbWNeNcsd9K6KxeRvK+3A1h78K9VCBw0VJSpIgEoCpDBn204zhOMwoDqgUfhZFJAgl2gzRTJX2z8zFKNHorpGnS+CZIOenGY7zfNC4HtL8pM1K5H9ofe+A2o7xN4j6d/MRutsSrMuk/Ry3Rw2O70Wu5bTOc2t0sfgRd8L23GewgCIV5vGHEX7XVZI/c+9Y0JA/G+z4nTTpcOixujVqfSpSFFD2bHIqVsfoTLStnVPrOLNgJqJaLO1MrdyY8JqLQxnXC0JlmjnkHvYabk9XZVHAjwfrmRnphnvbT1rh3MBDpQykcyts5UaV2x4O1fR9ovlubbxL4FUSeO073+tZghImwizFJKQh9qMww1zGDJMg48In3AtDD20+t01ru2HePR80fevuCe03MD+I3DJyqoVdaV18Xap1p4N55wozTaGwSorcVcNFaQzGK10VHTMXmx9uB6y7qcUuUmUyIWGeuzbUv6MGPHhhRwo2Q/TTrNy7iffqOeec3cnEVbUu6P7k284799gc78z6dLunMc7TmEPEMSEyxX7ICRZJIHEcc/BJEgUko2gzPNJQcGYMzqpCmMFcarNWunhX0v9DSZRkQATNcBz7HPsiI1h4aYRFHAgmSRCygPQoKSbR6QQmVW7BDMamhEJJ9fNI6b+onfbFt/o/1pIrhX/mbehypeRgBiCXqFdJO9mUbZQDcY2xNxqMHyphlRxMi9L9f1C6GEwHv1z/Rn99pfT8hIOUKfZSkWGfpwlOgIU4IlkELEslo6xXevGZJiZWUFhtqtXgBhbaalV2v6V3/b3rr2r0F1Iay4S7RY2F2A8I4BgoYMH9lDKPZSlrFrUGd5vOnR8z7H0ejPsJdf+/BjxJWEIZBhZz7CeSYk4BcMqklGmWZH5A0eZfwUJCGdgUAAA="
        },
        [484] = new List<string> { },
        [485] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1WS2/bOBD+KwbPEqAH9aBurjdJDThpUGfRQ9ADLY5kIozoklS32cL/fUFJtC0/kiLIcU+iZobffEMOP/I3mrZGzqg2elbVqPiNrhq6EjAVAhVGteAh61zwBvZO5lxzhoooJx66V1wqbl5QEXporq9+laJlwPZmG7/tsW6lLNcWrBtEdtThpLmHbjYPawV6LQVDRRgEI+TXoTsMko1mBG+Sma3b5wuF4TDAbzByIFIIKI2rBIdBeBgWvc1CKsapuEAkjNJ0tMZ4mHbN9frqBfRB4uSIcZKMGKduD+gTLNe8Mp8o73hbg3aGpaHlk0ZFMqxqmp/iHqKSAfWeGg5NCQd80uN56XgFIzdV8X9hRk3fGefaLM1PwKKj7YgHsIc1FZw+6Wv6UyqLNzK46mJvbP8KpfwJChWhXbMLFPAooVvOT7y+oc9d3dOmFqC0S2L3nqEizgJ8wn4ElW+3Hrr6ZRQdzqHdiAe5/Idu5o1pueGyuaG8cWvrhx5atApuQWtaAyoQ8tBdRwLdyQaQ1yO8bAAVdmHO4C2kNu/Gu1eg4TxD5KML/j5j59/zWW6gNIqKWasUNOaDqjxC/bBaz7I9qfhs9i6qb5ClkRt7fHlTLw1sOt3ccx+aaKo+hvIhXMfh74b/aMHiogoCnBKS+QkJwMeYEZ9kOfhRTCAKgdAkzdDWQwuuzZfK5tCoeOzb0xawI0jIZYZTISb91GOad1I9U/FZyicL5ATkG9Du39o1mN1ZrKjQ4M7m4LQFulM6mHp4HGZWmBzm0ijZ1B+AGsQHqAuooWFUvexl6w8R/pLtShxX2kdEKdkFnNA+DRlxOBP1oPjmUqYsieJdyKVco6BXsg1xtrenlQE1o229Ngv+bO+YsHccN333umhVf4nZwYE8n9HgOEvI6a38ygVrXwZOfVybfYUfLVfAloaa1t5z9ulxoff+rJfe7o3/W+BdLTB/554fKFxQkiBjceQHrEp9zGjkUyDYB6BpUCVAk3KFtt+dxA3P08edoVc5+99r6qBpjzhPvk+mxeSaN+DXijKYfKMG1OSaC/u5tT+cCj2ZjyU3T+KoJKvYD2EV+zgIA5+ymPl5UmUrjDHLK4q2/wFfn526mgsAAA=="
        },
        [486] = new List<string> { },
        [487] = new List<string> { },
        [488] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XbW/bNhD+KwY/bYBUiJJIifrmek4awMmCOms3BP1ASyebiCy6FNUlC/zfB+rFlmQ5wYoMGLB+k4/H55473puf0bTUcsYLXczSNYqe0TznqwymWYYirUqwkDlciByOh0l7dJWgyA2ZhW6VkEroJxRhC10V88c4KxNIjmKjv6+xrqWMNwas+nDNV4VDQwtd7u42CoqNzBIUYcfpIb8MXWGwoHfDeZXMbFNuzzjmY8cfMKKkz6gFkVkGsW498bGDu2ru6yykSgTPzhDBLqW9GPvNtQtRbOZPUHQMkwFj0mdM2zfgD7DciFS/56LibQRFK1hqHj8UKCJNVGl4ittFZQ3qLdcC8hg6fOjwHu1H0G2vKvEXzLiuM2MszWh4AuYOEsRrwO42PBP8objg36QyeD1B651n9eUfIZbfQKEIm5idoeD3DLbhfC/Wl3xb+T3N1xmoojVi3j5BkRc4/gn7HlS431to/qgVb+rQPMSdXP7Jd1e5LoUWMr/kIm9ja2MLLUoF11AUfA0oQshCNxUJdCNzQFaN8LQDFJnAjOAtZKG/G+9WQQHjDJGNzpzXFqvzI5/lDmKteDYrlYJcv5GXA9Q383WU7YnHo9YrrTpBllruTPmKfL3UsKv65pF7k0RT9TaUu3AVh99y8bUEg4s8jBkLA2YHzHFt3/FW9iohge06nDPqcYh9F+0ttBCF/jU1NgoU3dfpaRw4EGTsPMNplk3qq0OaN1JtefZBygcD1DaQz8Cr33URmtMCtPGkLUcjuhNbUIMyvRb54cjU5zvHQtf8sSNzHSMzbX/svpH3Mdg72oh7MNg1MA2z2h0fu0FoIt84sdRK5us3cKOmPHCD/DM3ar4jfpBX/VjAGvKEq6dXXTmB+EWWq+zwmj0Vl7KDwjFSZ1V6JEa07pTYnbMUENc7qJyz1VN6wVqjZ+p3mmpQM16uN3ohtmaO4vpgWNjVBlWqelCbj84IqqcDYcNV48Xdxaw7bUtta+cjfC2FgmSpuS7N8Db71LCgBq8UsNF8HWg53rlsGFP88eb/2psP2naIaWCn1A1t33GZzR0KtruKCYtTkjAI0f5L27ebnfv+IKhb9/0z6vZwn4QkPN/FZ3LFMz2pbnT7OH4pNlcJ5FrEPDMBMYZqhelWlnlHbWz/Jmy4Mnn9bda0mGWpUh7DMjOtqHWDkVc2RbK30H/mf8hx7n/3tDeXjWRmoloFtDv/m6lvPmvxUW0sdTtpBjSMU+5xm64wsX3XX9mrMHRs8DCJU57EdEXQ3jpNI3regUue8ViLeHKRcQXpj2z632RTGmDqhYFrOxRC2+cstVnoEzvAKfaJi73YrZtWjdtQvPfD8Mtk/ns0mUl4FIUW+Xpi9msBxeRq8tOH+fTTHxOVr3/ub7YBB0q479sB57HtY8JtlvrExsxJEicBgISi/d+6FnboAREAAA=="
        },
        [489] = new List<string> { },
        [490] = new List<string> { },
        [491] = new List<string> { },
        [492] = new List<string> { },
        [493] = new List<string> { },
        [494] = new List<string> { },
        [495] = new List<string> { },
        [508] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACrVW72+bPBD+VyJ/hgnzK8C3LMu6SklXNak2qdoHA0dileDUNn3bt8r/PhlwCiRZsir7Bne+x89zvjv7DY1KycZESDHOlih6Q5OCxDmM8hxFkpdgIOWc0gLenal2XacosoPQQLecMk7lK4qwga7F5CXJyxTSd7Nav62xZowlKwVWfdjqq8LxAwNdbRYrDmLF8hRF2LI6yH+GrjDCYSfCOklmvCrXmoGLLbdHwe5R6LhHMXsGFGUkFzs4lueQyBYibsfbp/kwnlKSH8k1tn2/k223CftKxWryCqK1sdeT4nkdKb4+DfII8xXN5GdCK97KILRhLknyKFDkNfn1g33cNmrYoN4SSaFIoMXH78f53dTaOpTT/2FMZF0jelf/xME4TfRiRXJKHsVX8sy4AugYtBzH6NrvIGHPwFGEVZIOFbkfqNpobajz95kur8i6EjoqljlwoTdRh52iyBla7h77DlSw3Rpo8iI56bTgjoA6iAWb/0c214UsqaSsuCK00OkxsYGmJYcZCEGWgCKEDHRTcUI3rADUILxuAEUqTwfwpkzID+PdchBwmCEy0RF/vWPlf+cz30AiOcnHJedQyAup7KFeTOtBtnuKD+5erbpTi8asLCTwOkKt16deV9Ncso1qblos5xI21Qh6V9ZU3IhfRlAbrmJ4X9CnEhQuCkmAIXESM85SbLp+aptBTDwTBw4OUjz0wbXR1kBTKuT3TO0hUPTwVu2mBOwIhuFxhqM8H9ShfZo3jK9J/o2xRwWkx8sPINW/sguQu76pRrJu5MbZzm1jquFdPFRjS2POJWfF8gKoltNCncISipTw10sB3wv4wspmvV5YW05kpYNm+0p7HXdS+dHIPXULugbeG2MzWuxcKMLuJ89AM/LStgWfrIO73AtYcLrpiq0tfyV26Nkqd3XkX8rtxH5EsLMvrsFUTT7KJPAxKZcrOaVrdRXj2tHv/uo5VvL6rlcfe/eNF/afMX98CKm3k57KusHu4KmkHNK5JLJU9796nB3puvO66Oyu6Cw8WNBnVO4ZJdpbdbCaziqbs+rjH5258/Ezb832xM5cPwhiE0PqmG4AsRmAF5phmDk4GRLs2DHa/tLDvXnAP+wM9XxX//Vt0kzzB88Kfg0mP6PBLWcJCAHpYPRUEkmTwQwkydWYb9EAYvnuMAzNIYSJ6QJJzDiAxIyTEDthnDrBMEPb38287c6yDAAA"
        },
        [509] = new List<string> 
        {
            "AH4_H4sIAAAAAAAACu1XTW/jNhD9KwbPIiBRpETp5nWz2QBOGsQpWiDYAyWNbCKK6KWo7aYL//eC+rAlxx/YwIeiyE0mZ968GT3NjH+iaW3UTFSmmuVLFP9EV6VICpgWBYqNrsFB9nIuS9hdZv3VTYZiwiMH3WuptDSvKPYcdFNd/UiLOoNsd2ztNy3WrVLpyoI1D8Q+NTgBd9D1+nGloVqpIkOx57oj5NPQDUYUjjzcs2Rmq/rlSGLUc+kZRj2IKgpITZ8J9VxvaEbOs1A6k6I4QsQjQTCqMe3cPstqdfUK1SAw22PM2Ihx0L8D8QyLlczNJyEb3vag6g8WRqTPFYpZV9WAv8UdokYd6r0wEsoUBnyCfb9gXEHSu2r5D8yEaZXRR933Jnv19zvvx5UopHiuPovvSluA0UGfju+Mzx8gVd9Bo9izRTok7YBbCQwC9vX7JJfX4qVJdFouC9BVH8S+7AzFfujSN+xHUHyzcdDVD6NF9+HZyj+qxd9ifVOaWhqpymshy74e2HPQvNZwC1UlloBihBx015BAd6oE5LQIr2tAsS3MAby5qsy78e41VHCYIcLoyH0bsbnf8VmsITVaFLNaayjNhbLcQ71YrgfZvsn4YPTGqhXIwqi1/V5luVwYWDeNcse9E9FUX4byEK7h8Ecpv9VgcZHrh74nKMEeZRGmXgpYsMzHqS8Yp1HKgEVo46C5rMzvuY1RofipladNYEswio4znBbFpHXdp3mn9Isovij1bIH6jvEniOa3Pa/AbL/FXBQV9N9md2kT7L/S7qiFp15oO1GPuTBalYORdt7d9Qfuc1hCmQn9egFeDfBvqk6K/UxbCxJEW4Md7aMmh6gNrR61XB+LFDLib02OxRoZnYjW2VltT3MDeibq5crM5YsdKl57sS/6Zp2odTu17MOgPR/owX7Iordj+MREtatA3316mT3At1pqyBZGmNoONrtrHNHeGS39qmQ+JPBrEnjvOx90uIwnlCU8w9SNAkxzn2OekxSnIIAQj6QR52jztW9x3T76tD1ou9zTTzRsd5RFhBxveF/Uei3L5WRel8vcug37nneqQDcZlEamorBVsdFag+mLqsuB2aEFlUX7K4Y/Xve4DVzrXKSwKGx32ubCzmxWbOOg/8yivpuT756O1tmezGxVm4IO52U3Je1je7wzO6TfgdaI4EJASDBEUY6pl2Q4AUpxELqBK4DylHG0cd5q6UQCt0qVhTSTmdDri+vo3cI5LMAPHV1GRwJ84kcEMOUuwZSSFCc0oTgJojCNMhcyHhzSkXdiCZvXpdCTByHFixgvYh9C+t8KKfUDFhGfYe7ngClNPMxBEAxeIrgIKCfgNsOvxe0oPjE3+jq5+iuePEAuS8gmtglNrqEY/3kgTBA/yn3b3yimJEswDxKOcw4sZDx08zxEm38BqMvUJVUSAAA="
        },
        [510] = new List<string> { },
        [511] = new List<string> { },

        #endregion

        #region Critical

        [542] = new List<string> { "" },
        [543] = new List<string> { "" },
        [544] = new List<string> { "" },

        #endregion
    };
}
