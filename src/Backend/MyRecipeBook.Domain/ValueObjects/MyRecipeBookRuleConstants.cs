﻿namespace MyRecipeBook.Domain.ValueObjects
{
    public static class MyRecipeBookRuleConstants
    {
        public const int MAXIMUM_INGREDIENTS_GENERATE_RECIPE = 5;
        public const int MAXIMUM_IMAGE_URL_LIFETIME_IN_MINUTES = 10;
        public const string CHAT_MODEL = "gpt-4.1-mini";
        public const int REFRESH_TOKEN_EXPIRATION_DAYS = 7;
    }
}
