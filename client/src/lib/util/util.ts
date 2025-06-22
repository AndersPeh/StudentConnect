import { format, type DateArg } from "date-fns";
import z from "zod";

// it takes argument of type DateArg (accepts date as string, number, Javascript date object) 
// then date-fns converts the argument to <Date>, a Javascript Date object.
export function formatDate(date: DateArg<Date>){
    return format(date, 'dd MMM yyyy h:mm a');
};

// set required condition for textfield to be used in schemas.
export const requiredString = (fieldName: string) => z
    .string({required_error: `${fieldName} is required`})

    .min(1, {message: `${fieldName} is required`});