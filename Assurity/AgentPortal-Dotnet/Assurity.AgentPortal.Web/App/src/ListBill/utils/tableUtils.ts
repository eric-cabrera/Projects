import type { Row } from "@/components/aTable/definitions";
import type {
  Assurity_ListBill_Service_Contracts_Group as Group,
  Assurity_ListBill_Service_Contracts_ListBill as ListBill,
} from "@assurity/newassurelink-client";

export const formatGroupsData = (groupData: Group[]) => {
  if (!groupData) {
    return [] as Row[];
  }

  return groupData.map(
    (item) =>
      [
        {
          key: "id",
          text: item.id,
          onClickReturnValue: item.id,
        },
        {
          key: "name",
          text: item.name,
        },
        {
          key: "city",
          text: item.city,
        },
        {
          key: "state",
          text: item.state,
        },
      ] as Row,
  );
};

export const formatListBillData = (listBillData: ListBill[]) => {
  if (!listBillData) {
    return [] as Row[];
  }

  return listBillData.map((item) => [
    {
      key: "document",
      text: `${item.id} - ${new Date(item?.date ?? "").toLocaleDateString("en-US")}`,
      onClickReturnValue: item.id,
    },
  ]) as Row[];
};
