using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace MultiworldPylon.Content
{
    public class MultiworldPylonItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MultiworldPylonTile>(), 0);
            Item.value = Item.sellPrice(0, 17, 25, 75);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.ShimmerTransformToItem[ItemID.TeleportationPylonVictory] = Item.type;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.RodofDiscord, 1)
                .AddIngredient(ItemID.GravitationPotion, 10)
                .AddIngredient(ItemID.SoulofLight, 20)
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddIngredient(ItemID.TeleportationPylonPurity, 1)
                .AddIngredient(ItemID.TeleportationPylonDesert, 1)
                .AddIngredient(ItemID.TeleportationPylonHallow, 1)
                .AddIngredient(ItemID.TeleportationPylonJungle, 1)
                .AddIngredient(ItemID.TeleportationPylonMushroom, 1)
                .AddIngredient(ItemID.TeleportationPylonOcean, 1)
                .AddIngredient(ItemID.TeleportationPylonUnderground, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
