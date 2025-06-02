export const groupMockData = [
  {
    id: "1234567890",
    name: "Vault-Tec",
    city: "Lincoln",
    state: "NE",
  },
  {
    id: "0987654321",
    name: "Insurance Inc",
    city: "Chicago",
    state: "IL",
  },
  {
    id: "2143658709",
    name: "Listbill Llc",
    city: "New York",
    state: "NY",
  },
];

export const groupTransformedData = [
  [
    {
      key: "id",
      text: "1234567890",
      onClickReturnValue: "1234567890",
    },
    {
      key: "name",
      text: "Vault-Tec",
    },
    {
      key: "city",
      text: "Lincoln",
    },
    {
      key: "state",
      text: "NE",
    },
  ],
  [
    {
      key: "id",
      text: "0987654321",
      onClickReturnValue: "0987654321",
    },
    {
      key: "name",
      text: "Insurance Inc",
    },
    {
      key: "city",
      text: "Chicago",
    },
    {
      key: "state",
      text: "IL",
    },
  ],
  [
    {
      key: "id",
      text: "2143658709",
      onClickReturnValue: "2143658709",
    },
    {
      key: "name",
      text: "Listbill Llc",
    },
    {
      key: "city",
      text: "New York",
    },
    {
      key: "state",
      text: "NY",
    },
  ],
];

// note that the february date is -6 hrs, this is because of Daylight Saving Time,
// all these Dates are technically the same time.
// Changing the time to noon that way people running these test in PT dont get errors
export const listBillMockData = [
  {
    key: "1234567890",
    value: [
      {
        id: "1000000006",
        date: new Date("2024-07-16T12:00:00-05:00"),
      },
      {
        id: "1000000005",
        date: new Date("2024-06-16T12:00:00-05:00"),
      },
    ],
  },
  {
    key: "0987654321",
    value: [
      {
        id: "1000000004",
        date: new Date("2024-05-16T12:00:00-05:00"),
      },
      {
        id: "1000000003",
        date: new Date("2024-04-16T12:00:00-05:00"),
      },
    ],
  },
  {
    key: "2143658709",
    value: [
      {
        id: "1000000002",
        date: new Date("2024-03-16T12:00:00-05:00"),
      },
      {
        id: "1000000001",
        date: new Date("2024-02-16T12:00:00-06:00"),
      },
    ],
  },
];

export const listBillTransformedData = [
  {
    key: "1234567890",
    value: [
      [
        {
          key: "document",
          text: "1000000006 - 7/16/2024",
          onClickReturnValue: "1000000006",
        },
      ],
      [
        {
          key: "document",
          text: "1000000005 - 6/16/2024",
          onClickReturnValue: "1000000005",
        },
      ],
    ],
  },
  {
    key: "0987654321",
    value: [
      [
        {
          key: "document",
          text: "1000000004 - 5/16/2024",
          onClickReturnValue: "1000000004",
        },
      ],
      [
        {
          key: "document",
          text: "1000000003 - 4/16/2024",
          onClickReturnValue: "1000000003",
        },
      ],
    ],
  },
  {
    key: "2143658709",
    value: [
      [
        {
          key: "document",
          text: "1000000002 - 3/16/2024",
          onClickReturnValue: "1000000002",
        },
      ],
      [
        {
          key: "document",
          text: "1000000001 - 2/16/2024",
          onClickReturnValue: "1000000001",
        },
      ],
    ],
  },
];
