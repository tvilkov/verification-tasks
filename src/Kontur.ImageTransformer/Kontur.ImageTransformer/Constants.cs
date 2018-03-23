namespace Kontur.ImageTransformer
{
    internal abstract class Constants
    {
        public const int MAX_IMAGE_SIZE = 100 * 1024 * 1024; //  100 Kb
        public const int MAX_IMAGE_HEIGHT = 1000;
        public const int MAX_IMAGE_WIDTH = 1000;

        /*
         Per YandexTank tests with ammo.txt (can be found in Solution Items).
         1. Simple HttpListener-based service which only answers 200 status code (without any image processing) ensures 2ms response time with up to 600 rps. 
            From 600 rps the reponse time starts growing exponentially reaching ~20ms (tenfold rise!) on 950-1000rps.
         2. Similar trend was observed with real image-processing HttpListener. 
            200RPS is an impirical threshold which allows to keep 95% of request processing time bellow 500ms and survive 10xRPS burst spikes with <=900ms delay.
         */
        public const int MAX_RPS = 200;
    }
}