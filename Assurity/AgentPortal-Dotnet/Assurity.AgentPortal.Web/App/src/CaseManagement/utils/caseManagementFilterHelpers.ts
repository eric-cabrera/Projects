import dayjs from "dayjs";

import type { aFilterItem } from "@/components/aFilterDrawer/models/aFilterItem";
import { aFilterField } from "@/components/aFilterDrawer/models/enums/aFilterField";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import type { CaseManagementFilterOptions } from "../models/CaseManagementFilterOptions";
import { CaseManagementFilterData } from "../models/CaseManagementFilterData";
import type { CaseManagementFilterItems } from "../models/CaseManagementFilterItems";
import { ProductType } from "../models/enums/ProductType";

class CaseManagementFilterHelpers {
  private filterModel: CaseManagementFilterData;

  constructor() {
    this.filterModel = new CaseManagementFilterData();
  }

  mapFilterValues(filter: aFilterData, filterItems: CaseManagementFilterItems) {
    switch (filter.field) {
      case aFilterField.PrimaryInsuredName:
        return filterItems?.primaryInsuredValues || [];
      case aFilterField.ProductTypes:
        return filterItems?.productTypeValues || [];
      case aFilterField.EventTypes:
        return filterItems.statusValues || [];
    }
  }

  resetFilters(filterGroup: aFilterData[]) {
    filterGroup.map((filter) => {
      if (filter.items) {
        filter.items = [];
      }

      if (filter.createdDateBegin) {
        filter.fromDate = null;
      }

      if (filter.toDate) {
        filter.toDate = null;
      }

      return filter;
    });
  }

  mapCreatedDateSelections = (
    createdDateFilter: aFilterData | null | undefined,
  ) => {
    const filterOptions: CaseManagementFilterOptions = {};
    if (
      createdDateFilter === null ||
      createdDateFilter === undefined ||
      createdDateFilter.createdDateBegin === undefined ||
      createdDateFilter.createdDateEnd === undefined ||
      createdDateFilter.createdDateBegin?.toString() === "Invalid Date" ||
      createdDateFilter.createdDateEnd?.toString() === "Invalid Date"
    ) {
      filterOptions.createdDateBegin = "";
      filterOptions.createdDateEnd = "";
    } else {
      filterOptions.createdDateBegin = dayjs(
        createdDateFilter.createdDateBegin,
      ).format("MM/DD/YYYY");
      filterOptions.createdDateEnd = dayjs(
        createdDateFilter.createdDateEnd,
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
    let primaryInsuredName: (string | null | undefined) | undefined = "";
    let productTypes: (string | null | undefined)[] | undefined = [];
    let eventTypes: (string | null | undefined)[] | undefined = [];
    let viewAsAgentId: string | null | undefined = "";
    filterSelections.map((filterSelection) => {
      const ids = filterSelection?.selection?.map(
        (selection: aFilterItem) => selection?.id,
      );
      switch (filterSelection.field) {
        case aFilterField.ViewAsAgent:
          viewAsAgentId = ids?.at(0);
          break;
        case aFilterField.PrimaryInsuredName:
          primaryInsuredName = ids?.at(0);
          break;
        case aFilterField.EventTypes:
          eventTypes = ids;
          break;
        case aFilterField.ProductTypes:
          productTypes = ids;
          break;
      }
    });

    const filterOptions: CaseManagementFilterOptions = {
      primaryInsuredName: primaryInsuredName,
      productTypes: productTypes
        .join(";")
        .replace(
          ProductType.IncomeProtection,
          ProductType.IncomeProtectionAccidentOnly +
            ";" +
            ProductType.IncomeProtectionAccidentSickness, //Income Protection needs to filter by both of these products.
        )
        .replace(
          ProductType.Term,
          ProductType.TermLife + ";" + ProductType.TermDeveloperEdition, //Term needs to filter by both of these products.
        ),
      eventTypes: eventTypes.join(";"),
      viewAsAgentId: viewAsAgentId,
    };
    return filterOptions;
  };

  getFilterSelections(
    filterData: CaseManagementFilterData | undefined,
    filterGroup: aFilterData[],
    filterOptions: CaseManagementFilterOptions,
    filters: CaseManagementFilterItems,
  ): aFilterData[] | null {
    const filterModel: CaseManagementFilterData =
      new CaseManagementFilterData();

    if (!filterData || !filterOptions) return null;

    if (filterGroup.length === 0) {
      filterGroup = filterModel.caseManagementFilters;
    }

    filterGroup = filterGroup.map((filter) => {
      switch (filter.field) {
        case aFilterField.PrimaryInsuredName:
          filter.items = this.generateFilter(filters.primaryInsuredValues);
          break;
        case aFilterField.ProductTypes:
          filter.items = this.generateFilter(filters?.productTypeValues || []);
          break;
        case aFilterField.EventTypes:
          filter.items = this.generateFilter(filters?.statusValues || []);
          filter.items;
          break;
      }
      return filter;
    });

    return filterGroup;
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
export default new CaseManagementFilterHelpers();
