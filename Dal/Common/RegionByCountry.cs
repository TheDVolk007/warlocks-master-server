using Domain;


namespace Dal.Common
{
    // TODO: Переделать в нестатический класс и выделить интерфейс к тому моменту, как класс будет использоваться по полной
    public static class RegionByCountry
    {
        public static Region GetClosestRegion(string country)
        {
            return Region.NorthEurope; // DEBUG ONLY
        }
    }
}