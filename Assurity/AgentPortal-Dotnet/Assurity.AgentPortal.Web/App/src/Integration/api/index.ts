import axios from "axios";
import type { Assurity_AgentPortal_Contracts_AgentContracts_DropdownOption as DropdownOption } from "@assurity/newassurelink-client";

export const individualForms = async (): Promise<string> => {
  try {
    const response = await axios.get<string>("/forms/GetIndividualForms/");

    // Check if the response status is in the 2xx range
    if (response.status >= 200 && response.status < 300) {
      return response.data;
    } else {
      throw new Error(`Unexpected response status: ${response.status}`);
    }
  } catch (error) {
    console.error("Failed to fetch individual forms:", error);
    throw new Error(
      "Unable to retrieve individual forms. Please try again later.",
    );
  }
};

export const worksiteForms = async (): Promise<string> => {
  try {
    const response = await axios.get<string>("/forms/GetWorksiteForms/");

    // Check if the response status is in the 2xx range
    if (response.status >= 200 && response.status < 300) {
      return response.data;
    } else {
      throw new Error(`Unexpected response status: ${response.status}`);
    }
  } catch (error) {
    console.error("Failed to fetch worksite forms:", error);
    throw new Error(
      "Unable to retrieve worksite forms. Please try again later.",
    );
  }
};

export const getViewAsAgents = async (): Promise<DropdownOption[]> => {
  const response = await axios.get<DropdownOption[]>(
    "/API/UserData/ViewAsAgents",
  );

  if (response.status === 200) {
    return response.data;
  }

  return [];
};

export const getWPSUrl = async (): Promise<string> => {
  try {
    const response = await axios.get<string>("/WPS/GetWPSUrl");

    // Check if the response status is in the 2xx range
    if (response.status >= 200 && response.status < 300) {
      return response.data;
    } else {
      throw new Error(`Unexpected response status: ${response.status}`);
    }
  } catch (error) {
    console.error("Failed to fetch WPS Url:", error);
    throw new Error("Unable to retrieve WPS Url. Please try again later.");
  }
};

export const getIllustrationProUrl = async (): Promise<string> => {
  try {
    const response = await axios.get<string>(
      "/IllustrationPro/GetIllustrationProUrl",
    );

    // Check if the response status is in the 2xx range
    if (response.status >= 200 && response.status < 300) {
      return response.data;
    } else {
      throw new Error(`Unexpected response status: ${response.status}`);
    }
  } catch (error) {
    console.error("Failed to fetch Illustration Pro Url:", error);
    throw new Error(
      "Unable to retrieve Illustration Pro Url. Please try again later.",
    );
  }
};

export const setCookiesAndGetAwpsUrl = async (
  input: string,
): Promise<string | undefined> => {
  try {
    const response = await axios.get<string>("/AWPS/SetCookiesAndGetAwpsUrl", {
      params: { input },
    });

    if (response.status >= 200 && response.status < 300) {
      const urlAwps = response.data;
      const link = document.createElement("a");
      link.href = urlAwps;
      link.target = "_blank";
      link.click();
    } else {
      throw new Error(`Unexpected response status: ${response.status}`);
    }
  } catch (error) {
    return undefined;
  }
};
