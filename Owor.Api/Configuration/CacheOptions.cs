namespace Owor.Api.Configuration
{

    public class CacheOptions
    {

        public int AbsoluteExpiration { get; set; } = 120;

        public int SlidingExpiration { get; set; } = 30;

    }

}