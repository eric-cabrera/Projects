import axios from "axios";
import type { CreatePasswordResponse } from "../models/createPasswordResposne";

export const createPassword = async (
  email: string,
  activationId: string,
  password: string,
): Promise<CreatePasswordResponse> => {
  return await axios
    .post("/API/Subaccount/ActivateSubaccount", {
      email,
      activationId,
      password,
    })
    .then((response) => {
      if (response.status && response.status >= 200 && response.status < 300) {
        return response.data;
      }
      return {} as CreatePasswordResponse;
    })
    .catch(() => {
      return {} as CreatePasswordResponse;
    });
};
