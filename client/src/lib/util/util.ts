import { format, type DateArg } from "date-fns";

// it takes argument of type DateArg (accepts date as string, number, Javascript date object) 
// then date-fns converts the argument to <Date>, a Javascript Date object.
export function formatDate(date: DateArg<Date>){
    return format(date, 'dd MMM yyyy h:mm a');
};