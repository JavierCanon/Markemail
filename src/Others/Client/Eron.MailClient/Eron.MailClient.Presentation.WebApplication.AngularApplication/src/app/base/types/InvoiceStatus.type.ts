export enum InvoiceStatusType {
  WaitingForPayment = 0,
  Received = 1,
  Processing = 2,
  Posting = 3,
  Posted = 4,
  Delivered = 5,
  Canceled = 6,
  CanceledByAdmin = 7,
  NeedInvoice = 8
}
