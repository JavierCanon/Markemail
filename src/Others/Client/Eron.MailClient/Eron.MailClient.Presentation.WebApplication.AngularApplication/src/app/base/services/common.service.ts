
export class CommonService {

  private defaultTableSettings: any = {
    confirmDelete: true,
    attr: {
      class: 'table table-stripped'
    },
    noDataMessage: 'رکوردی یافت نشد',
    actions: {
      columnTitle: 'عملیات',
      position: 'right'
    },
    add: {
      addButtonContent: 'جدید',
      createButtonContent: 'ثبت',
      cancelButtonContent: 'انصراف',
      confirmButtonContent: 'تایید و ثبت'
    },
    edit: {
      editButtonContent: 'ویرایش',
      saveButtonContent: 'ذخیره',
      cancelButtonContent: 'انصراف'
    },
    delete: {
      deleteButtonContent: 'حذف'
    }
  };

  private defaultTableRequestSettings: any= {
    filterFieldKey: 'filter',
    pagerLimitKey: 'pageSize',
    totalKey: 'totalCount',
    pagerPageKey: 'pageNumber',
    sortDirKey: 'order',
    sortFieldKey: 'orderBy'
  };

  getTableSettings() {
    return this.defaultTableSettings;
  }

  getTableRequestSettings(): any {
    return this.defaultTableRequestSettings;
  }
}
