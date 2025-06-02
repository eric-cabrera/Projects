import dayjs from "dayjs";

import type { aFilterItem } from "@/components/aFilterDrawer/models/aFilterItem";
import { aFilterField } from "@/components/aFilterDrawer/models/enums/aFilterField";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import type { GroupInventoryFilterOptions } from "../models/GroupInventoryFilterOptions";
import { GroupInventoryFilterData } from "../models/GroupInventoryFilterData";
import type {
  Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupSummaryFilters as GroupSummaryFilters,
  Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupDetailFilters as GroupDetailFilters,
} from "@assurity/newassurelink-client";
import type { GroupInventoryDetailFilterOptions } from "../models/GroupInventoryDetailFilterOptions";

class GroupInventoryFilterHelpers {
  private filterModel: GroupInventoryFilterData;

  constructor() {
    this.filterModel = new GroupInventoryFilterData();
  }

  resetFilters(filterGroup: aFilterData[]) {
    filterGroup.map((filter) => {
      if (filter.items) {
        filter.items = [];
      }

      if (filter.effectiveDateStart) {
        filter.fromDate = null;
      }

      if (filter.toDate) {
        filter.toDate = null;
      }

      return filter;
    });
  }

  mapEffectiveDateSelections = (
    effectiveDateFilter: aFilterData | null | undefined,
  ) => {
    const filterOptions: GroupInventoryFilterOptions = {};
    if (
      effectiveDateFilter === null ||
      effectiveDateFilter === undefined ||
      effectiveDateFilter.effectiveDateStart === undefined ||
      effectiveDateFilter.effectiveDateEnd === undefined ||
      effectiveDateFilter.effectiveDateStart?.toString() === "Invalid Date" ||
      effectiveDateFilter.effectiveDateEnd?.toString() === "Invalid Date"
    ) {
      filterOptions.groupEffectiveStartDate = "";
      filterOptions.groupEffectiveEndDate = "";
    } else {
      filterOptions.groupEffectiveStartDate = dayjs(
        effectiveDateFilter.effectiveDateStart,
      ).format("MM/DD/YYYY");
      filterOptions.groupEffectiveEndDate = dayjs(
        effectiveDateFilter.effectiveDateEnd,
      ).format("MM/DD/YYYY");
    }
    return filterOptions;
  };

  mapSelections = (
    filterSelections: {
      field: string;
      selection: aFilterItem[] | null | undefined;
    }[],
  ) => {
    let groupNameOrNumber: string | null | undefined = "";
    let groupStatus: (string | null | undefined)[] | undefined = [];

    filterSelections.map((filterSelection) => {
      const ids = filterSelection?.selection?.map(
        (selection: aFilterItem) => selection?.id,
      );
      switch (filterSelection.field) {
        case aFilterField.GroupNameOrNumber:
          groupNameOrNumber = ids?.at(0);
          break;
        case aFilterField.GroupStatus:
          groupStatus = ids;
          break;
      }
    });

    const filterOptions: GroupInventoryFilterOptions = {
      groupNumber: groupNameOrNumber,
      groupStatus: groupStatus ? groupStatus.join(";") : undefined,
    };
    return filterOptions;
  };

  mapDetailSelections = (
    filterSelections: {
      field: string;
      selection: aFilterItem[] | null | undefined;
    }[],
  ) => {
    let policyOwnerName: string | null | undefined = "";
    let policyNumber: string | null | undefined = "";
    let policyStatus: (string | null | undefined)[] | undefined = [];
    let productDescription: (string | null | undefined)[] | undefined = [];

    filterSelections.forEach((filterSelection) => {
      const ids = filterSelection?.selection?.map(
        (selection: aFilterItem) => selection?.id,
      );
      switch (filterSelection.field) {
        case aFilterField.PolicyOwnerName:
          policyOwnerName = ids?.at(0);
          break;
        case aFilterField.PolicyNumber:
          policyNumber = ids?.at(0);
          break;
        case aFilterField.PolicyStatus:
          policyStatus = ids;
          break;
        case aFilterField.ProductDescription:
          productDescription = ids;
          break;
      }
    });

    const filterOptions: GroupInventoryDetailFilterOptions = {
      policyOwnerName: policyOwnerName,
      policyNumber: policyNumber,
      policyStatus: policyStatus ? policyStatus.join(";") : undefined,
      productDescription: productDescription
        ? productDescription.join(";")
        : undefined,
    };
    return filterOptions;
  };

  getFilterItems(
    filterGroup: aFilterData[],
    filterOptions: GroupInventoryFilterOptions,
    filters: GroupSummaryFilters | undefined,
  ): aFilterData[] | null {
    const filterModel: GroupInventoryFilterData =
      new GroupInventoryFilterData();

    if (!filterOptions) return null;

    if (filterGroup.length === 0) {
      filterGroup = filterModel.groupInventoryFilters;
    }

    filterGroup = filterGroup.map((filter) => {
      const groupNamesAndNumbers = filters
        ? filters.groupNamesAndNumbers?.map((item) => {
            return { text: item.displayValue, id: item.number } as {
              text: string;
              id: string;
            };
          })
        : [];

      switch (filter.field) {
        case aFilterField.GroupNameOrNumber:
          filter.items = this.generateIdFilter(groupNamesAndNumbers);
          break;
        case aFilterField.GroupStatus:
          filter.items = this.generateFilter(filters?.groupStatusValues || []);
          break;
      }
      return filter;
    });

    return filterGroup;
  }

  getDetailFilterItems(
    filterGroup: aFilterData[],
    filterOptions: GroupInventoryDetailFilterOptions,
    filters: GroupDetailFilters | undefined,
  ): aFilterData[] | null {
    const filterModel: GroupInventoryFilterData =
      new GroupInventoryFilterData();

    if (!filterOptions) return null;

    if (filterGroup.length === 0) {
      filterGroup = filterModel.groupInventoryFilters;
    }

    filterGroup = filterGroup.map((filter) => {
      switch (filter.field) {
        case aFilterField.PolicyOwnerName:
          filter.items = this.generateFilter(filters?.policyOwners || []);
          break;
        case aFilterField.PolicyNumber:
          filter.items = this.generateFilter(filters?.policyNumbers || []);
          break;
        case aFilterField.PolicyStatus:
          filter.items = this.generateFilter(filters?.policyStatuses || []);
          break;
        case aFilterField.ProductDescription:
          filter.items = this.generateFilter(
            filters?.productDescriptions || [],
          );
          break;
      }
      return filter;
    });

    return filterGroup;
  }

  private generateIdFilter(
    filterValues: { text: string; id: string }[] | undefined,
  ): aFilterItem[] {
    const items = filterValues
      ? filterValues.map(
          (item: { text: string; id: string }) =>
            <aFilterItem>{
              id: item.id,
              text: item.text,
              option: "",
              level: 0,
            },
        )
      : [];
    return items;
  }

  private generateFilter(filterValues: string[] | undefined): aFilterItem[] {
    const items = filterValues
      ? filterValues.map(
          (item: string) =>
            <aFilterItem>{
              id: item,
              text: item,
              option: "",
              level: 0,
            },
        )
      : [];
    return items;
  }
}

export default new GroupInventoryFilterHelpers();
