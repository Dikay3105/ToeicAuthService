namespace AuthService.Helper
{
    public static class RNG
    {
        public static int GenerateSixDigitNumber()
        {
            Random random = new Random();
            return random.Next(100000, 1000000); // Tạo số ngẫu nhiên từ 100000 đến 999999
        }
    }
}
