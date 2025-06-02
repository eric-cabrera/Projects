import dayjs from "dayjs";

export const getAgeByDOB = (dateOfBirthString: string | null | undefined) => {
  const today = dayjs();
  const birthdate = dayjs(dateOfBirthString);
  const age = today.diff(birthdate, "year");

  // Check if the birthday has already occurred this year
  if (
    today.month() < birthdate.month() ||
    (today.month() === birthdate.month() && today.date() < birthdate.date())
  ) {
    return age - 1; // Subtract 1 if the birthday hasn't occurred yet this year
  }
  return age;
};
