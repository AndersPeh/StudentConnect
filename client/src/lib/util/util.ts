import { format, formatDistanceToNow, type DateArg } from "date-fns";
import z from "zod";

// it takes argument of type DateArg (accepts date as string, number, Javascript date object)
// then date-fns converts the argument to <Date>, a Javascript Date object.
export function formatDate(date: DateArg<Date>) {
  return format(date, "dd MMM yyyy h:mm a");
}

// timeAgo takes a date input and uses formatDistanceToNow to calculate the distance
// between the given date and the current time. Append the string ago to the result,
// so it will display e.g. 2 days ago.
export function timeAgo(date: DateArg<Date>) {
  return formatDistanceToNow(date) + " ago";
}

// set required condition for textfield to be used in schemas.
export const requiredString = (fieldName: string) =>
  z
    .string({ required_error: `${fieldName} is required` })

    .min(1, { message: `${fieldName} is required` });
