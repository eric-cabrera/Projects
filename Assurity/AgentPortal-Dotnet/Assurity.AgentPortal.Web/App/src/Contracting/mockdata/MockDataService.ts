export type AgentContract = {
  AgentLevel: string;
  AgentNumber: string;
  MarketCode: string;
  CompanyNumber: string;
};

export type AgentsContractsResponse = {
  AgentContracts: AgentContract[];
};

export class MockDataService {
  static getAgentContracts: () => AgentsContractsResponse = () => {
    return {
      AgentContracts: [
        {
          AgentLevel: "90",
          AgentNumber: "AAXB",
          MarketCode: "WS-1",
          CompanyNumber: "01",
        },
        {
          AgentLevel: "90",
          AgentNumber: "AAXB",
          MarketCode: "WSR5",
          CompanyNumber: "01",
        },
      ],
    };
  };
}
