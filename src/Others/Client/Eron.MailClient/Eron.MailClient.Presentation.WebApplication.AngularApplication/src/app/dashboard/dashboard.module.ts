import { NgModule } from '@angular/core';
import { BaseModule } from '../base/base.module';
import { EmailRootComponent } from '../Dashboard/email-root/email-root.component';
import { SettingsRootComponent } from '../Dashboard/settings-root/settings-root.component';
import { TicketRootComponent } from '../Dashboard/ticket-root/ticket-root.component';
import { InsightRootComponent } from '../Dashboard/insight-root/insight-root.component';
import { BreadCrumpComponent } from './layout/bread-crump/bread-crump.component';
import { FooterComponent } from './layout/footer/footer.component';
import { HeaderComponent } from './layout/header/header.component';
import { RightSideComponent } from './layout/right-side/right-side.component';
import { SideComponent } from './layout/side/side.component';
import { SkinConfigComponent } from './layout/skin-config/skin-config.component';

@NgModule({
  declarations: [
    // Layout Components
    BreadCrumpComponent,
    FooterComponent,
    HeaderComponent,
    RightSideComponent,
    SideComponent,
    SkinConfigComponent,

    // Root Components
    EmailRootComponent,
    SettingsRootComponent,
    TicketRootComponent,
    InsightRootComponent
  ],
  imports: [BaseModule],
  providers: []
})
export class DashboardModule {}
