import type { Row } from "@/components/aTable/definitions";

export const itemsWithNestedHeaders = [
  { key: "line-of-business", text: "Line of Business" },
  { key: "annualized-premium", text: "Annualized Premium" },
  { key: "policy-count", text: "Policy Count" },
] as Row;

export const itemsWithNestedItems = [
  [
    {
      key: "line-of-business",
      text: "1- Life",
    },
    {
      key: "annualized-premium",
      text: "400,000",
    },
    {
      key: "policy-count",
      text: "1421",
    },
    {
      nestedRow: [
        [
          {
            key: "line-of-business",
            text: "2 - Term Life",
            appendIcon: "termLifeIcon",
          },
          {
            key: "annualized-premium",
            text: "200,000",
          },
          {
            key: "policy-count",
            text: "1000",
          },
          {
            nestedRow: [
              [
                {
                  key: "line-of-business",
                  text: "3 - Term Life - 10 year",
                },
                {
                  key: "annualized-premium",
                  text: "90,000",
                },
                {
                  key: "policy-count",
                  text: "101",
                },
              ],
              [
                {
                  key: "line-of-business",
                  text: "3 - Term Life - 20 year",
                },
                {
                  key: "annualized-premium",
                  text: "90,000",
                },
                {
                  key: "policy-count",
                  text: "101",
                },
              ],
            ],
          },
        ],
        [
          {
            key: "line-of-business",
            text: "2 - Whole Life",
            appendIcon: "wholeLifeIcon",
          },
          {
            key: "annualized-premium",
            text: "200,000",
          },
          {
            key: "policy-count",
            text: "421",
          },
          {
            nestedRow: [
              [
                {
                  key: "line-of-business",
                  text: "3 - Whole Life - 10 year",
                },
                {
                  key: "annualized-premium",
                  text: "90,000",
                },
                {
                  key: "policy-count",
                  text: "101",
                },
              ],
              [
                {
                  key: "line-of-business",
                  text: "3 - Whole Life - 20 year",
                },
                {
                  key: "annualized-premium",
                  text: "90,000",
                },
                {
                  key: "policy-count",
                  text: "101",
                },
              ],
            ],
          },
        ],
      ],
    },
  ],
  [
    {
      key: "line-of-business",
      text: "1 - Health",
    },
    {
      key: "annualized-premium",
      text: "200,000",
    },
    {
      key: "policy-count",
      text: "1421",
    },
    {
      nestedRow: [
        [
          {
            key: "line-of-business",
            text: "2 - Hospital Indemnity",
          },
          {
            key: "annualized-premium",
            text: "200,000",
          },
          {
            key: "policy-count",
            text: "1421",
          },
        ],
        [
          {
            key: "line-of-business",
            text: "2 - Disability Income",
          },
          {
            key: "annualized-premium",
            text: "25,000",
          },
          {
            key: "policy-count",
            text: "39",
          },
        ],
        [
          {
            key: "line-of-business",
            text: "2 - Critical Illness",
          },
          {
            key: "annualized-premium",
            text: "10,000",
          },
          {
            key: "policy-count",
            text: "21",
          },
        ],
        [
          {
            key: "line-of-business",
            text: "2 - Accident",
          },
          {
            key: "annualized-premium",
            text: "15,000",
          },
          {
            key: "policy-count",
            text: "21",
          },
          {
            nestedRow: [
              [
                {
                  key: "line-of-business",
                  text: "3 - Group Accident Expense",
                },
                {
                  key: "annualized-premium",
                  text: "5,000",
                },
                {
                  key: "policy-count",
                  text: "21",
                },
              ],
              [
                {
                  key: "line-of-business",
                  text: "3 - Group Accident Indemnity",
                },
                {
                  key: "annualized-premium",
                  text: "10,000",
                },
                {
                  key: "policy-count",
                  text: "21",
                },
              ],
              [
                {
                  key: "line-of-business",
                  text: "3 - Group Accident Medical",
                },
                {
                  key: "annualized-premium",
                  text: "15,000",
                },
                {
                  key: "policy-count",
                  text: "21",
                },
              ],
            ],
          },
        ],
      ],
    },
  ],
  [
    {
      key: "line-of-business",
      text: "1 - Auto",
    },
    {
      key: "annualized-premium",
      text: "200,000",
    },
    {
      key: "policy-count",
      text: "1421",
    },
    {
      nestedRow: [
        [
          {
            key: "line-of-business",
            text: "2 - Auto",
            appendIcon: "autoIcon",
          },
          {
            key: "annualized-premium",
            text: "200,000",
          },
          {
            key: "policy-count",
            text: "1421",
          },
          {
            nestedRow: [
              [
                {
                  key: "line-of-business",
                  text: "3 - Auto - 10 year",
                },
                {
                  key: "annualized-premium",
                  text: "90,000",
                },
                {
                  key: "policy-count",
                  text: "101",
                },
              ],
            ],
          },
        ],
      ],
    },
  ],
] as Row[];

export const downlineHeaders: Row = [
  { key: "agency", text: "Agency" },
  { key: "policy-count", text: "Policy Count", align: "right" },
  {
    key: "annual-premium",
    text: "Annualized Premium",
    align: "right",
    margin: "left",
  },
  { key: "percent-change", text: "% Change", align: "right", isSortable: true },
];

export const downlineItems = [
  [
    { key: "agency", text: "Empower Brokerage", link: "https://google.com" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Affinity Marketing Group" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Afg Marketing" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Yoder and Associates" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Premier Financial Services" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Pheonix Marking Group" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Extreme Insurance Team" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Homeloader Protection" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
  [
    { key: "agency", text: "Oaktree Financial" },
    { key: "policy-count", text: 400, align: "right" },
    {
      key: "annual-premium",
      text: 303984,
      align: "right",
    },
  ],
];

export const paginatedItems = [
  ...downlineItems,
  ...downlineItems,
  ...downlineItems,
];

export const writingAgentsHeaders = [
  { key: "agent", text: "Agent" },
  { key: "policy-count", text: "Policy Count" },
  { key: "annual-premium", text: "Annualized Premium" },
];

export const writingAgentsItems = [
  [
    { key: "agent", text: "Agent 1" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 2" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 3" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 4" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 5" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 6" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 7" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 8" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 9" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
  [
    { key: "agent", text: "Agent 10" },
    { key: "policy-count", text: "Policy Count" },
    {
      key: "annual-premium",
      text: "Annualized Premium",
      appendIcon: "downArrowIcon",
    },
  ],
];

export const policyDetailHeaders = [
  { key: "agent-name", text: "Agent Name - ID" },
  { key: "policy-number", text: "Policy Number" },
  { key: "transaction-type", text: "Transaction Type" },
  { key: "line-of-business", text: "Line of Business" },
  { key: "product-description", text: "Product Description" },
  { key: "insured-name", text: "Insured Name" },
  { key: "transaction-date", text: "Transaction Date" },
  { key: "application-date", text: "Application Date" },
  { key: "mode", text: "Mode" },
  { key: "mode-premium", text: "Mode Premium" },
  { key: "policy-count", text: "Policy Count" },
  { key: "annual-premium", text: "Annualized Premium" },
];

export const policyDetailItems = [
  [
    { key: "agent-name", text: "Agent 1", subText: "123456" },
    { key: "policy-number", text: "123456" },
    { key: "transaction-type", text: "Submitted" },
    { key: "line-of-business", text: "Life" },
    { key: "product-description", text: "Term Life" },
    { key: "insured-name", text: "John Doe" },
    { key: "transaction-date", text: "01/01/2021" },
    { key: "application-date", text: "01/01/2021" },
    { key: "mode", text: "Annual" },
    { key: "mode-premium", text: "1000" },
    { key: "policy-count", text: "1" },
    { key: "annual-premium", text: "1000" },
  ],
  [
    { key: "agent-name", text: "Agent 2", subText: "123456" },
    { key: "policy-number", text: "123456" },
    { key: "transaction-type", text: "Submitted" },
    { key: "line-of-business", text: "Life" },
    { key: "product-description", text: "Term Life" },
    { key: "insured-name", text: "John Doe" },
    { key: "transaction-date", text: "01/01/2021" },
    { key: "application-date", text: "01/01/2021" },
    { key: "mode", text: "Annual" },
    { key: "mode-premium", text: "1000" },
    { key: "policy-count", text: "1" },
    { key: "annual-premium", text: "1000" },
  ],
  [
    { key: "agent-name", text: "Agent 3", subText: "123456" },
    { key: "policy-number", text: "123456" },
    { key: "transaction-type", text: "Submitted" },
    { key: "line-of-business", text: "Life" },
    { key: "product-description", text: "Term Life" },
    { key: "insured-name", text: "John Doe" },
    { key: "transaction-date", text: "01/01/2021" },
    { key: "application-date", text: "01/01/2021" },
    { key: "mode", text: "Annual" },
    { key: "mode-premium", text: "1000" },
    { key: "policy-count", text: "1" },
    { key: "annual-premium", text: "1000" },
  ],
  [
    { key: "agent-name", text: "Agent 4", subText: "123456" },
    { key: "policy-number", text: "123456" },
    { key: "transaction-type", text: "Submitted" },
    { key: "line-of-business", text: "Life" },
    { key: "product-description", text: "Term Life" },
    { key: "insured-name", text: "John Doe" },
    { key: "transaction-date", text: "01/01/2021" },
    { key: "application-date", text: "01/01/2021" },
    { key: "mode", text: "Annual" },
    { key: "mode-premium", text: "1000" },
    { key: "policy-count", text: "1" },
    { key: "annual-premium", text: "1000" },
  ],
];
export const downlineComparisonHeaders = [
  { key: "direct-downline", text: "Direct Downline" },
  {
    key: "2023-annualized-premium",
    text: ["[2023]", "Annualized Premium"],
    align: "right",
    margin: "left",
  },
  {
    key: "2022-annualized-premium",
    text: ["[2022]", "Annualized Premium"],
    align: "right",
  },
  {
    key: "percent-change-annualized-premium",
    text: "% Change",
    align: "right",
  },
  {
    key: "2023-policy-count",
    text: ["[2023]", "Policy Count"],
    align: "right",
    margin: "left",
  },
  {
    key: "2022-policy-count",
    text: ["[2022]", "Policy Count"],
    align: "right",
  },
  { key: "percent-change-policy-count", text: "% Change", align: "right" },
];

export const downlineComparisonItems = [
  [
    { key: "direct-downline", text: "Empower Brokerage" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    {
      key: "percent-change-annualized-premium",
      text: 0,
      align: "right",
      prependIcon: "upArrowIcon",
    },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Affinity Marketing Group" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Afg Marketing" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Yoder and Associates" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Premier Financial Services" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Pheonix Marking Group" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Extreme Insurance Team" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Homeloader Protection" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
  [
    { key: "direct-downline", text: "Oaktree Financial" },
    { key: "2023-annualized-premium", text: 303984, align: "right" },
    { key: "2022-annualized-premium", text: 303984, align: "right" },
    { key: "percent-change-annualized-premium", text: 0, align: "right" },
    { key: "2023-policy-count", text: 400, align: "right" },
    { key: "2022-policy-count", text: 400, align: "right" },
    { key: "percent-change-policy-count", text: 0, align: "right" },
  ],
];

export const supplementalData = [
  {
    name: "Empower Brokerage",
    policyCount: 400,
    premium: 303984,
  },
];

export const supplementalItems = [
  [
    { isBold: true, key: "name", text: "Empower Brokerage" },
    { key: "policy-count", text: "400", sortValue: 400, align: "right" },
    {
      key: "annual-premium",
      text: "$303,984.00",
      sortValue: 303984.0,
      align: "right",
    },
  ],
];

export const policyDetailData = [
  {
    agentId: "4BJ5",
    agentName: "MUZQUIZ, HECTOR SOLIZ",
    annualPremium: 143565.72,
    insuredName: "GARZA, RAMIRO",
    lineOfBusiness: "Life",
    mode: "Monthly",
    modePremium: 11963.81,
    policyCount: 1,
    policyNumber: "4351426805",
    productType: "Whole Life",
    productDescription: "Assurity Whole Life",
    applicationDate: "2023-09-18T00:00:00",
    transactionDate: "2023-12-07T00:00:00",
    transactionType: "DECLINED",
  },
];

export const policyDetailTableItems = [
  [
    {
      className: "policy-detail-table__cell__agent",
      key: "name",
      text: "muzquiz, hector soliz",
      sortValue: "muzquiz, hector soliz",
      subText: "4BJ5",
    },
    {
      key: "policy-number",
      text: "4351426805",
      sortValue: "4351426805",
    },
    {
      key: "transaction-type",
      text: "declined",
      sortValue: "declined",
    },
    {
      key: "line-of-business",
      text: "Life",
    },
    {
      key: "product-description",
      text: "Assurity Whole Life",
    },
    {
      key: "insured-name",
      text: "garza, ramiro",
    },
    {
      key: "transaction-date",
      text: "12/07/2023",
      sortValue: "12/07/2023",
    },
    {
      key: "application-date",
      text: "09/18/2023",
      sortValue: "09/18/2023",
    },
    {
      key: "mode",
      text: "Monthly",
    },
    {
      key: "mode-premium",
      text: "$11,963.81",
      align: "right",
    },
    {
      key: "policy-count",
      text: "1",
      align: "right",
    },
    {
      key: "annual-premium",
      text: "$143,565.72",
      sortValue: 143565.72,
      align: "right",
    },
  ],
];

export const individualProductsData = [
  {
    grouping: "Lines of Business",
    agentId: null,
    name: "Life",
    premiumCurrent: 488233.1,
    premiumPrevious: null,
    premiumChangePercent: null,
    policyCountCurrent: 298,
    policyCountPrevious: null,
    policyCountChangePercent: null,
    children: [
      {
        grouping: "Products",
        agentId: null,
        name: "Universal Life",
        premiumCurrent: 68596.94,
        premiumPrevious: null,
        premiumChangePercent: null,
        policyCountCurrent: 31,
        policyCountPrevious: null,
        policyCountChangePercent: null,
      },
    ],
  },
];

export const individualProductsItems = [
  [
    {
      isBold: true,
      key: "name",
      text: "life",
      sortValue: "Life",
    },
    {
      key: "premium-current",
      text: "$488,233.10",
      sortValue: 488233.1,
      align: "right",
    },
    {
      key: "policy-current",
      text: "298",
      sortValue: 298,
      align: "right",
    },
    {
      nestedRow: [
        [
          {
            isBold: true,
            key: "name",
            text: "universal life",
            sortValue: "Universal Life",
          },
          {
            key: "premium-current",
            text: "$68,596.94",
            sortValue: 68596.94,
            align: "right",
          },
          {
            key: "policy-current",
            text: "31",
            sortValue: 31,
            align: "right",
          },
        ],
      ],
    },
  ],
];

export const individualProductsTotalsData = {
  productionByDownline: [],
  productionByDownlineCount: 0,
  productionByProduct: [],
  productionByProductCount: 0,
  totalPremiumCurrent: 1608939.78,
  totalPolicyCountCurrent: 1105,
  supplementalReports: [],
  filters: {},
};

export const individualProductsTotals = [
  [
    {
      key: "name",
      text: "Totals",
      isHeading: true,
      isBold: true,
    },
    {
      key: "premium-current",
      text: "$1,608,939.78",
      align: "right",
      isBold: true,
    },
    {
      key: "policy-current",
      text: "1,105",
      align: "right",
      isBold: true,
    },
  ],
];

export const worksiteProductsData = [
  {
    grouping: "Lines of Business",
    agentId: null,
    name: "Life",
    premiumCurrent: 756.96,
    premiumPrevious: 756.96,
    premiumChangePercent: 0,
    policyCountCurrent: 298,
    policyCountPrevious: 298,
    policyCountChangePercent: 0,
    children: [
      {
        grouping: "Products",
        agentId: null,
        name: "Universal Life",
        premiumCurrent: 68596.94,
        premiumPrevious: 68596.94,
        premiumChangePercent: 0,
        policyCountCurrent: 248,
        policyCountPrevious: 248,
        policyCountChangePercent: 0,
      },
    ],
  },
];

export const worksiteProductsItems = [
  [
    {
      isBold: true,
      key: "name",
      text: "life",
      sortValue: "Life",
    },
    {
      key: "premium-current",
      text: "$756.96",
      sortValue: 756.96,
      align: "right",
    },
    {
      key: "premium-previous",
      text: "$756.96",
      sortValue: 756.96,
      align: "right",
    },
    {
      key: "premium-change-percent",
      text: "0%",
      sortValue: 0,
      percentChange: 0,
      align: "right",
    },
    {
      key: "policy-current",
      text: "298",
      sortValue: 298,
      align: "right",
    },
    {
      key: "policy-previous",
      text: "298",
      sortValue: 298,
      align: "right",
    },
    {
      key: "policy-change-percent",
      text: "0%",
      percentChange: 0,
      sortValue: 0,
      align: "right",
    },
    {
      nestedRow: [
        [
          {
            isBold: true,
            key: "name",
            text: "universal life",
            sortValue: "Universal Life",
          },
          {
            key: "premium-current",
            text: "$68,596.94",
            sortValue: 68596.94,
            align: "right",
          },
          {
            key: "premium-previous",
            text: "$68,596.94",
            sortValue: 68596.94,
            align: "right",
          },
          {
            key: "premium-change-percent",
            text: "0%",
            percentChange: 0,
            sortValue: 0,
            align: "right",
          },
          {
            key: "policy-current",
            text: "248",
            sortValue: 248,
            align: "right",
          },
          {
            key: "policy-previous",
            text: "248",
            sortValue: 248,
            align: "right",
          },
          {
            key: "policy-change-percent",
            text: "0%",
            percentChange: 0,
            sortValue: 0,
            align: "right",
          },
        ],
      ],
    },
  ],
];

export const worksiteProductsTotalsData = {
  productionByAgent: [],
  productionByGroup: [],
  productionByProduct: [],
  totalGroupCountCurrent: 7,
  totalPolicyCountCurrent: 35,
  totalPremiumCurrent: 9693.04,
  productionAgentBySupplementalReport: {},
  filters: {},
};

export const worksiteProductsTotals = [
  [
    {
      key: "name",
      text: "Totals",
      isHeading: true,
      isBold: true,
    },
    {
      key: "premium-current",
      text: "$9,693.04",
      align: "right",
      isBold: true,
    },
    {
      key: "premium-previous",
      text: "$0.00",
      align: "right",
      isBold: true,
    },
    {
      key: "premium-change-percent",
      text: "0%",
      percentChange: 0,
      align: "right",
      isBold: true,
    },
    {
      key: "policy-current",
      text: "35",
      align: "right",
      isBold: true,
    },
    {
      key: "policy-previous",
      text: "0",
      align: "right",
      isBold: true,
    },
    {
      key: "policy-change-percent",
      text: "0%",
      percentChange: 0,
      align: "right",
      isBold: true,
    },
  ],
];

export const agentData = [
  {
    grouping: "Writing Agents",
    agentId: "H339",
    agentName: null,
    name: "HEBERT, TIMOTHY J",
    effectiveDate: null,
    groupNumber: null,
    groupCountCurrent: 4,
    groupCountPrevious: 0,
    groupCountChangePercent: 0,
    policyCountCurrent: 28,
    policyCountPrevious: 0,
    policyCountChangePercent: 0,
    premiumCurrent: 82,
    premiumPrevious: 0,
    premiumChangePercent: 0,
  },
];

export const agentItems = [
  [
    {
      isBold: true,
      key: "name",
      subText: "H339",
      text: "hebert, timothy j",
    },
    {
      key: "group-count-current",
      text: "4",
      sortValue: 4,
      align: "right",
    },
    {
      key: "group-count-previous",
      text: "0",
      sortValue: 0,
      align: "right",
    },
    {
      key: "group-count-change-percent",
      text: "0%",
      percentChange: 0,
      sortValue: 0,
      align: "right",
    },
    {
      key: "policy-current",
      text: "28",
      sortValue: 28,
      align: "right",
    },
    {
      key: "policy-previous",
      text: "0",
      sortValue: 0,
      align: "right",
    },
    {
      key: "policy-change-percent",
      text: "0%",
      sortValue: 0,
      percentChange: 0,
      align: "right",
    },
    {
      key: "premium-current",
      text: "$82.00",
      sortValue: 82,
      align: "right",
    },
    {
      key: "premium-previous",
      text: "$0.00",
      sortValue: 0,
      align: "right",
    },
    {
      key: "premium-change-percent",
      text: "0%",
      percentChange: 0,
      sortValue: 0,
      align: "right",
    },
  ],
];
