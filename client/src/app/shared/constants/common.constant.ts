import { RoutingConfig } from "../../models/base/routing-config.model";
import { ActionExponent } from "../enumerations/permission.enum";

export class CommonConstant {
  public static DISALLOW_NOTICE = "dn";
  public static ALLOW_NOTICE_WITH_MSG_BOX = "alwsm";
  public static ALLOW_NOTICE_WITH_SNACKBAR_WARNING = "alwsw";
  public static ALLOW_NOTICE_WITH_SNACKBAR_DANGER = "alwsd";
  public static readonly ZERO_GUID = "00000000-0000-0000-0000-000000000000";
}

/**
 * Danh sách routing
*/
export class Routing {
  public static readonly NOT_FOUND = new RoutingConfig('404', 'NOT_FOUND');
  public static readonly ACCESS_DENIED = new RoutingConfig('403', 'ACCESS_DENIED');
  public static readonly UNSUPPORT_DEVICE = new RoutingConfig('unsupport-device', 'UNSUPPORT_DEVICE');
  public static readonly TOKEN_RECEIVER = new RoutingConfig('token', 'TOKEN_RECEIVER');
  public static readonly SIGN_IN = new RoutingConfig('sign-in', 'SIGN_IN');
  public static readonly SIGN_UP = new RoutingConfig('sign-up', 'SIGN_UP');
  public static readonly SIGN_OUT = new RoutingConfig('sign-out', 'SIGN_OUT');
  public static readonly SIGN_IN_HISTORY = new RoutingConfig('sign-in-history', 'SIGN_IN_HISTORY');
  public static readonly CPANEL = new RoutingConfig('cpanel', 'CPANEL', [ActionExponent.SA]);
  public static readonly AUDIT_LOG = new RoutingConfig('audit-logs', 'AUDIT_LOG', []);
  public static readonly NOTEBOOK = new RoutingConfig('notebook', 'NOTEBOOK');
  public static readonly NOTEBOOK_VIEW_MODE = new RoutingConfig('notebook/view-mode', 'NOTEBOOK_VIEW_MODE');
  public static readonly DIRECTORY = new RoutingConfig('cloud-storage', 'DIRECTORY', [ActionExponent.Cloud]);
  public static readonly INFORMATION = new RoutingConfig('ri', 'INFORMATION');
  public static readonly FEEDBACK = new RoutingConfig('feedback', 'FEEDBACK');
  public static readonly SESSION_LOG = new RoutingConfig('session-logs', 'SESSION_LOG');
  public static readonly CHAT_GENERATOR = new RoutingConfig('chat-generator', 'CHAT_GENERATOR', [ActionExponent.ChatGenerator]);
  public static readonly GENERATE_HISTORY = new RoutingConfig('chat-generator/history', 'GENERATE_HISTORY', [ActionExponent.ChatGenerator]);
  public static readonly GENERATE_CHAT = new RoutingConfig('chat-generator/generate', 'GENERATE_CHAT', [ActionExponent.ChatGenerator]);
}

export const CommonRedirect = Routing.DIRECTORY.path;

export class BreakPoint {
  public static SM = 576;
  public static MD = 768;
  public static LG = 992;
  public static XL = 1200;
  public static XXL = 1400;
}
