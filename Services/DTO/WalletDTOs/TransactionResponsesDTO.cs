﻿namespace Services.DTO.WalletDTOs
{
    public class TransactionResponsesDTO
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string TransactionType { get; set; }
        public Int64 Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
    }
}
