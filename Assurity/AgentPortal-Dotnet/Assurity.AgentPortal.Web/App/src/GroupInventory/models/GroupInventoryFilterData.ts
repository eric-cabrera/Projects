import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import type { aFilterItem } from "@/components/aFilterDrawer/models/aFilterItem";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import { GroupInventoryFilterField } from "./GroupInventoryFilterField";

export class GroupInventoryFilterData {
  public groupInventoryFilters: aFilterData[] = [
    {
      display: true,
      text: "Effective Date",
      type: aFilterType.EffectiveDates,
      field: GroupInventoryFilterField.EffectiveDates,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
    {
      display: true,
      text: "Group Name or Number",
      type: aFilterType.AutoCompleteStaticItems,
      field: GroupInventoryFilterField.GroupNameOrNumber,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
    {
      display: true,
      text: "Status",
      type: aFilterType.ProductionFilterItems,
      field: GroupInventoryFilterField.GroupStatus,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
      isTiered: true,
      isMultiple: true,
    },

    {
      display: false,
      text: "View As Agent",
      type: aFilterType.ViewAsAgent,
      field: GroupInventoryFilterField.ViewAsAgent,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
  ];
}
