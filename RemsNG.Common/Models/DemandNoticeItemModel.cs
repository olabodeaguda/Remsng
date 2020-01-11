using System;

namespace RemsNG.Common.Models
{
    public class DemandNoticeItemModelExt : DemandNoticeItemModel
    {
        public string TaxpayerName { get; set; }
        public string AddressName { get; set; }
        public string itemCode { get; set; }
    }
    public class DemandNoticeItemModel
    {
        public Guid Id { get; set; }
        public long BillingNo { get; set; }
        public Guid DnTaxpayersDetailsId { get; set; }
        public Guid TaxpayerId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string ItemStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string category { get; set; }
        public string wardName { get; set; }
        public Guid DemandNoticeId { get; set; }


        public virtual DemandNoticeTaxpayersModel DnTaxpayersDetails { get; set; }
        public virtual ItemModel Item { get; set; }
    }

    public enum DemandNoticeItemStatus
    {
        CANCEL, PAID, PENDING, PART_PAYMENT
    }
}
