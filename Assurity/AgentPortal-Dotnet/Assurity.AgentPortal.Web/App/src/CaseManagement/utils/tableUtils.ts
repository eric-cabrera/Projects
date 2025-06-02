import { type Cell, type Row } from "@/components/aTable/definitions";
import dayjs from "dayjs";
import {
  type Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementCase as Case,
  type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Child as Child,
} from "@assurity/newassurelink-client";
import { ComponentName } from "../models/enums/ComponentName";
import { ProductType } from "../models/enums/ProductType";
import { AudienceType } from "../models/enums/AudienceType";
import { EventType } from "../models/enums/EventType";

const ActionType = { Resume: "Resume", ResendEmail: "Resend Email", None: "" };

export const getMainRow = (
  item: Case,
  isSubaccount: boolean = false,
  isHomeOfficeImpersonating: boolean = false,
) => {
  let actionName = ActionType.None;
  let cellTooltip = "";
  const currentEvent = item.currentEvent;
  const caseFirstEvent = item.events?.at(-1);
  const expireDate = dayjs(currentEvent?.createdDateTime).utc().add(14, "day");
  const expireDays = Math.floor(expireDate.diff(dayjs(), "day", true));
  const expirePhrase = `expire in ${expireDays} days on ${expireDate.format("MM/DD/YYYY")}`;
  const expiredCase = expireDays < 0;
  const expiredText = "Expired";
  const applicationSubmittedText = "Application Submitted";
  const applicationSubmitted =
    currentEvent?.eventName === applicationSubmittedText;
  const signatureExpiredText = "Signature Expired";
  const signatureExpired = currentEvent?.eventName === signatureExpiredText;
  const signatureRequestedText = "Signature Requested";
  const signatureRequested = currentEvent?.eventName === signatureRequestedText;
  const statusText = applicationSubmitted
    ? applicationSubmittedText
    : signatureExpired || (signatureRequested && expiredCase)
      ? signatureExpiredText
      : expiredCase
        ? expiredText
        : (currentEvent?.eventName ?? "-");

  if (expireDays > 0 && expireDays <= 5) {
    if (currentEvent?.event === EventType.RecipientSent) {
      cellTooltip = `Signature request ${expirePhrase}`;
    } else if (currentEvent?.event === EventType.CaseStarted) {
      cellTooltip = `Case will ${expirePhrase}`;
    }
  }
  if (item.audience !== AudienceType.ConsumerFixedAgent && !expiredCase) {
    switch (currentEvent?.event) {
      case EventType.ReceivedQuote:
      case EventType.CaseStarted:
      case EventType.InterviewStarted:
      case EventType.InterviewCompleted:
        actionName = ActionType.Resume;
        break;
      case EventType.RecipientSent:
        actionName = ActionType.ResendEmail;
        break;
      default:
        break;
    }
  }

  const mainRow: Cell[] = [
    {
      key: "column-1",
      text:
        item.primaryInsured?.lastName + ", " + item.primaryInsured?.firstName,
    },
    { key: "column-2", text: item.productName ?? "-" },
    {
      key: "column-3",
      text: statusText,
      cellTooltip: cellTooltip,
    },
    {
      key: "column-4",
      text: dayjs(caseFirstEvent?.createdDateTime).utc().format("MM/DD/YYYY"),
    },
  ];

  if (actionName) {
    if (
      (actionName === ActionType.Resume &&
        !(isSubaccount || isHomeOfficeImpersonating)) ||
      (actionName === ActionType.ResendEmail && !isSubaccount)
    ) {
      mainRow.push({
        key: "column-5",
        text: actionName,
        onClickReturnValue: {
          eventType: item.currentEvent?.event,
          product: item.product,
          envelopeId: item.currentEvent?.envelopeId,
          experienceKey: item.experienceKey,
          cacheId: item.cacheId,
          quoteId: item.quoteId,
        },
        buttonStyle: "primary",
        buttonSize: "s",
      });
    }
  } else {
    mainRow.push({
      key: "column-5",
      text: actionName,
    });
  }
  return mainRow;
};

export const getStatusHistoryRow = (item: Case) => {
  const statusHistoryRow: Row = [
    { key: "column-1" },
    { key: "column-2" },
    {
      key: "column-3",
      componentName: ComponentName.StatusHistoryName,
      componentProps: {
        events: item.events,
        signers: item.signedInformation?.signers,
      },
    },
    {
      key: "column-4",
      componentName: ComponentName.StatusHistoryDate,
      componentProps: {
        events: item.events,
        signers: item.signedInformation?.signers,
      },
    },
    {
      key: "column-5",
      componentName: ComponentName.StatusHistoryAction,
      componentProps: {
        events: item.events,
        signers: item.signedInformation?.signers,
        audience: item.audience,
        product: item.product,
        experienceKey: item.experienceKey,
        cacheId: item.cacheId,
        quoteId: item.quoteId,
      },
    },
  ];
  return statusHistoryRow;
};

export const getStatusHistoryRowMobile = (item: Case) => {
  const statusHistoryRow: Row = [
    {
      key: "column-1",
      componentName: ComponentName.SectionHeading,
      componentProps: {
        heading: "Status History",
        className: "case-management__table__section-heading-mobile",
      },
    },
    {
      key: "column-1",
      componentName: ComponentName.StatusHistoryMobile,
      componentProps: {
        events: item.events,
      },
    },
  ];
  return statusHistoryRow;
};

export const getClientInfoRows = (item: Case) => {
  const clientInfoRows: Row[] = [];
  let clientInfoRow: Cell[] = [];
  clientInfoRow.push({
    key: "column-1",
    componentName: ComponentName.SectionHeading,
    componentProps: {
      heading: "Client Information",
      className: "case-management__table__section-heading",
    },
  });

  clientInfoRow.push({
    key: "column-2",
    componentName: ComponentName.PrimaryInsuredPerson,
    componentProps: {
      primaryInsured: item.primaryInsured,
      product: item.product,
    },
  });

  let columnPos = 3;
  if (item.spouseInsured) {
    clientInfoRow.push({
      key: "column-" + columnPos.toString(),
      componentName: ComponentName.SpousePerson,
      componentProps: {
        spouseInsured: item.spouseInsured,
        product: item.product,
      },
    });
    columnPos++;
  }

  if (item.children) {
    item.children?.forEach((child: Child) => {
      clientInfoRow.push({
        key: "column-" + columnPos.toString(),
        componentName: "ChildPerson",
        componentProps: {
          child: child,
          product: item.product,
        },
      });
      columnPos++;
      if (columnPos > 5) {
        columnPos = 2;
        clientInfoRows.push(clientInfoRow);
        clientInfoRow = [
          {
            key: "column-1",
          },
        ];
      }
    });
  }

  if (
    item.primaryPayor &&
    item.primaryPayor.relationshipToPrimaryInsured !== null &&
    item.primaryPayor.relationshipToPrimaryInsured !== "Self" &&
    !(
      item.primaryPayor?.firstName === item.primaryInsured?.firstName &&
      item.primaryPayor?.lastName === item.primaryInsured?.lastName &&
      item.primaryPayor?.phoneNumber === item.primaryInsured?.phoneNumber &&
      item.primaryPayor?.emailAddress === item.primaryInsured?.emailAddress
    )
  ) {
    clientInfoRow.push({
      key: "column-" + columnPos.toString(),
      componentName: "RelatedPerson",
      componentProps: {
        relatedPerson: item.primaryPayor,
        relationship: "Payor",
        case: item,
      },
    });
    columnPos++;
    if (columnPos > 5) {
      columnPos = 2;
      clientInfoRows.push(clientInfoRow);
      clientInfoRow = [
        {
          key: "column-1",
        },
      ];
    }
  }

  if (
    item.primaryOwner &&
    item.primaryOwner.relationshipToPrimaryInsured !== null &&
    item.primaryOwner.relationshipToPrimaryInsured !== "Self" &&
    !(
      item.primaryOwner?.firstName === item.primaryInsured?.firstName &&
      item.primaryOwner?.lastName === item.primaryInsured?.lastName &&
      item.primaryOwner?.phoneNumber === item.primaryInsured?.phoneNumber &&
      item.primaryOwner?.emailAddress === item.primaryInsured?.emailAddress
    )
  ) {
    clientInfoRow.push({
      key: "column-" + columnPos.toString(),
      componentName: "RelatedPerson",
      componentProps: {
        relatedPerson: item.primaryOwner,
        relationship: "Owner",
        case: item,
      },
    });
    columnPos++;
    if (columnPos > 5) {
      columnPos = 2;
      clientInfoRows.push(clientInfoRow);
      clientInfoRow = [
        {
          key: "column-1",
        },
      ];
    }
  }

  if (columnPos > 2) {
    while (columnPos < 6) {
      clientInfoRow.push({
        key: "column-" + columnPos.toString(),
      });
      columnPos++;
    }
    clientInfoRows.push(clientInfoRow);
  }

  return clientInfoRows;
};

export const getClientInfoRowsMobile = (item: Case) => {
  const clientInfoRows: Row[] = [];
  clientInfoRows.push([
    {
      key: "column-1",
      componentName: ComponentName.SectionHeading,
      componentProps: {
        heading: "Client Information",
        className: "case-management__table__section-heading-mobile",
      },
    },
    {
      key: "column-1",
      componentName: ComponentName.ClientSectionMobile,
      componentProps: {
        primaryInsured: item.primaryInsured,
        spouse: item.spouseInsured,
        children: item.children,
        primaryPayor: item.primaryPayor,
        primaryOwner: item.primaryOwner,
        product: item.product,
      },
    },
  ]);

  return clientInfoRows;
};

export const getProductInfoRow = (item: Case) => {
  const productInfoRow: Cell[] = [];

  productInfoRow.push(
    {
      key: "column-1",
      componentName: ComponentName.SectionHeading,
      componentProps: {
        heading: "Product Information",
        className: "case-management__table__section-heading",
      },
    },
    {
      key: "column-2",
      componentName: ComponentName.PrimaryInsuredProduct,
      componentProps: {
        coverages: item.primaryInsured?.coverages,
        productName: item.productName,
        product: item.product,
      },
    },
  );
  let columnPos = 3;
  if (item.spouseInsured) {
    productInfoRow.push({
      key: "column-" + columnPos.toString(),
      componentName: ComponentName.SpouseProduct,
      componentProps: {
        coverages: item.spouseInsured?.coverages,
        productName: item.productName,
        product: item.product,
      },
    });
    columnPos++;
  }

  const isAccidentalDeath = [
    ProductType.AccidentalDeath as string,
    ProductType.AccidentalDeathDismemberment as string,
  ].includes(item.product ?? "");
  const isTermLife = [
    ProductType.TermLife as string,
    ProductType.TermDeveloperEdition as string,
  ].includes(item.product ?? "");

  if (
    item.children &&
    item.children.length > 0 &&
    (isAccidentalDeath || isTermLife)
  ) {
    productInfoRow.push({
      key: "column-" + columnPos.toString(),
      componentName: ComponentName.ChildProduct,
      componentProps: {
        coverages: item.primaryInsured?.coverages,
        productName: item.productName,
        product: item.product,
        isAccidentalDeath: isAccidentalDeath,
        isTermLife: isTermLife,
      },
    });
    columnPos++;
  }

  if (columnPos > 2) {
    while (columnPos < 6) {
      productInfoRow.push({
        key: "column-" + columnPos.toString(),
      });
      columnPos++;
    }
  }

  return productInfoRow;
};

export const getProductInfoRowsMobile = (item: Case) => {
  const productInfoRows: Row[] = [];
  const hasChildren = (item.children?.length ?? 0) > 0;

  productInfoRows.push([
    {
      key: "column-1",
      componentName: ComponentName.SectionHeading,
      componentProps: {
        heading: "Product Information",
        className: "case-management__table__section-heading-mobile",
      },
    },
    {
      key: "column-1",
      componentName: ComponentName.ProductSectionMobile,
      componentProps: {
        baseCoverages: item.primaryInsured?.coverages,
        spouseCoverages: item.spouseInsured?.coverages,
        productName: item.productName,
        product: item.product,
        hasChildren: hasChildren,
      },
    },
  ]);

  return productInfoRows;
};
