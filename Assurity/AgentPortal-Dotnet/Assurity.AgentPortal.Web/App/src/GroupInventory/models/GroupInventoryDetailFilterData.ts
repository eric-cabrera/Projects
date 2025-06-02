import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import type { aFilterItem } from "@/components/aFilterDrawer/models/aFilterItem";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import { GroupInventoryDetailFilterField } from "./GroupInventoryDetailFilterField";

export class GroupInventoryDetailFilterData {
  public groupInventoryDetailsFilters: aFilterData[] = [
    {
      display: true,
      text: "Primary Owner",
      type: aFilterType.AutoCompleteStaticItems,
      field: GroupInventoryDetailFilterField.PolicyOwnerName,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
    {
      display: true,
      text: "Policy Number",
      type: aFilterType.AutoCompleteStaticItems,
      field: GroupInventoryDetailFilterField.PolicyNumber,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
    {
      display: true,
      text: "Policy Status",
      type: aFilterType.ProductionFilterItems,
      field: GroupInventoryDetailFilterField.PolicyStatus,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
      isTiered: true,
      isMultiple: true,
    },
    {
      display: true,
      text: "Issue Dates",
      type: aFilterType.CreatedDates,
      field: GroupInventoryDetailFilterField.IssueDate,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
    {
      display: true,
      text: "Product Description",
      type: aFilterType.ProductionFilterItems,
      field: GroupInventoryDetailFilterField.ProductDescription,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
      isTiered: true,
      isMultiple: true,
    },
  ];
}
