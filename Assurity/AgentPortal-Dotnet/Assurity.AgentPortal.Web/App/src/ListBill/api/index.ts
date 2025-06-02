import { groupMockData, listBillMockData } from "../mockdata/mockdata";
// note the functions in this file are placeholders until the actual api functions get hooked up

export function getGroups(
  agentId: string,
): { id: string; name: string; city: string; state: string }[] {
  if (agentId) {
    return groupMockData;
  }
  return [];
}

export function getListBills(groupId: string): { id: string; date: Date }[] {
  return listBillMockData.find((data) => data.key == groupId)?.value ?? [];
}
